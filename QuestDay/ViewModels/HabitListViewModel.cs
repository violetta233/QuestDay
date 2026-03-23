using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using QuestDay.Messages;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuestDay.ViewModels
{
    public partial class HabitListViewModel : ObservableObject, IRecipient<NewHabitMessage>
    {
        public ObservableCollection<Habit> Habits { get; } = new ObservableCollection<Habit>();

        private readonly IHabitService _habitService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadHabitsCommand))]
        private bool isBusy;

        public HabitListViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            WeakReferenceMessenger.Default.Register<NewHabitMessage>(this);

            Habits.CollectionChanged += (s, e) =>
            {
                Debug.WriteLine($"Коллекция Habits изменилась. Новое количество: {Habits.Count}");
            };
        }

        public async Task InitializeAsync()
        {
            await _habitService.InitializeAsync();
            await LoadHabitsCommand.ExecuteAsync(null);
        }

        public void Receive(NewHabitMessage message)
        {
            Debug.WriteLine($"Получено сообщение о новой привычке: {message.Value.Name}, Id: {message.Value.Id}");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!Habits.Any(h => h.Id == message.Value.Id))
                {
                    Habits.Add(message.Value);
                    Debug.WriteLine($"Привычка добавлена в коллекцию. Всего привычек: {Habits.Count}");
                }
                else
                {
                    Debug.WriteLine($"Привычка с Id {message.Value.Id} уже существует в коллекции");
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanLoadHabits))]
        private async Task LoadHabits()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                Debug.WriteLine("Начало загрузки привычек");
                Habits.Clear();
                var habitsFromDb = await _habitService.GetHabitsAsync();

                Debug.WriteLine($"Загружено из БД: {habitsFromDb.Count} привычек");

                foreach (var habit in habitsFromDb)
                {
                    Debug.WriteLine($"Добавляем привычку: {habit.Name}, Id: {habit.Id}, IsActive: {habit.IsActive}");
                    habit.IsCompletedForToday = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    Habits.Add(habit);
                }

                Debug.WriteLine($"После добавления в ObservableCollection: {Habits.Count} привычек");

                OnPropertyChanged(nameof(Habits));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки: {ex}");
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить привычки: {ex.Message}", "ОК");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLoadHabits() => !IsBusy;

        [RelayCommand]
        private async Task DeleteHabit(Habit habitToDelete)
        {
            if (habitToDelete == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Удалить привычку", $"Вы уверены, что хотите удалить '{habitToDelete.Name}'?", "Да", "Нет");
            if (confirm)
            {
                try
                {
                    await _habitService.DeleteHabitAsync(habitToDelete);
                    Habits.Remove(habitToDelete);
                    Debug.WriteLine($"Привычка '{habitToDelete.Name}' удалена");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка удаления: {ex}");
                    await Shell.Current.DisplayAlert("Ошибка", $"Не удалось удалить привычку: {ex.Message}", "ОК");
                }
            }
        }

        [RelayCommand]
        private async Task ToggleHabitIsActiveStatus(Habit habitToToggle)
        {
            if (habitToToggle == null) return;
            habitToToggle.IsActive = !habitToToggle.IsActive;
            try
            {
                await _habitService.UpdateHabitAsync(habitToToggle);
                Debug.WriteLine($"Статус активности привычки '{habitToToggle.Name}' изменен на: {habitToToggle.IsActive}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления статуса: {ex}");
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить статус активности привычки: {ex.Message}", "ОК");
            }
        }

        [RelayCommand]
        private async Task NavigateToAddHabit()
        {
            await Shell.Current.GoToAsync(nameof(AddPage));
        }

        [RelayCommand]
        private async Task ToggleHabitCompletion(Habit habitToToggleCompletion)
        {
            if (habitToToggleCompletion == null) return;
            habitToToggleCompletion.IsCompletedForToday = !habitToToggleCompletion.IsCompletedForToday;

            try
            {
                await _habitService.SaveHabitCompletionAsync(
                    habitToToggleCompletion.Id,
                    DateTime.Today,
                    habitToToggleCompletion.IsCompletedForToday
                );
                Debug.WriteLine($"Статус выполнения привычки '{habitToToggleCompletion.Name}' изменен на: {habitToToggleCompletion.IsCompletedForToday}");
            }
            catch (Exception ex)
            {
                habitToToggleCompletion.IsCompletedForToday = !habitToToggleCompletion.IsCompletedForToday;
                Debug.WriteLine($"Ошибка обновления выполнения: {ex}");
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить статус выполнения привычки: {ex.Message}", "ОК");
            }
        }
    }
}