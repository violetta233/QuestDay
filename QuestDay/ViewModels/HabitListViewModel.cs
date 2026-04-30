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
        private readonly IHouseStateService _houseStateService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadHabitsCommand))]
        private bool isBusy;

        public int HabitsCount => Habits.Count;

        public HabitListViewModel(IHabitService habitService, IHouseStateService houseStateService)
        {
            _habitService = habitService;
            _houseStateService = houseStateService;
            WeakReferenceMessenger.Default.Register<NewHabitMessage>(this);

            Habits.CollectionChanged += (s, e) =>
            {
                Debug.WriteLine($"Коллекция Habits изменилась. Новое количество: {Habits.Count}");
                OnPropertyChanged(nameof(HabitsCount));
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
                    Habits.Insert(0, message.Value);
                    SortHabits();
                    Debug.WriteLine($"Привычка добавлена в коллекцию. Всего привычек: {Habits.Count}");
                    OnPropertyChanged(nameof(HabitsCount));
                    await _houseStateService.UpdateStateAsync();
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

                SortHabits();

                Debug.WriteLine($"После добавления в ObservableCollection: {Habits.Count} привычек");

                OnPropertyChanged(nameof(Habits));
                OnPropertyChanged(nameof(HabitsCount));

                await _houseStateService.UpdateStateAsync();
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
                    OnPropertyChanged(nameof(HabitsCount));
                    await _houseStateService.UpdateStateAsync();
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

            bool previousState = habitToToggle.IsActive;
            habitToToggle.IsActive = !previousState;

            try
            {
                await _habitService.UpdateHabitAsync(habitToToggle);
                Debug.WriteLine($"Статус активности привычки '{habitToToggle.Name}' изменен на: {habitToToggle.IsActive}");
                await _houseStateService.UpdateStateAsync();
                SortHabits();
            }
            catch (Exception ex)
            {
                habitToToggle.IsActive = previousState;
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

            bool previousState = habitToToggleCompletion.IsCompletedForToday;
            habitToToggleCompletion.IsCompletedForToday = !previousState;

            try
            {
                await _habitService.SaveHabitCompletionAsync(
                    habitToToggleCompletion.Id,
                    DateTime.Today,
                    habitToToggleCompletion.IsCompletedForToday
                );

                SortHabits();

                Debug.WriteLine($"Статус выполнения привычки '{habitToToggleCompletion.Name}' изменен на: {habitToToggleCompletion.IsCompletedForToday}");

                var allHabits = await _habitService.GetHabitsAsync();
                var activeHabits = allHabits.Where(h => h.IsActive).ToList();

                bool allCompleted = true;
                foreach (var habit in activeHabits)
                {
                    bool isCompleted = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    if (!isCompleted)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                if (allCompleted && activeHabits.Count > 0)
                {
                    await _houseStateService.CleanHouseAsync();
                    await Shell.Current.DisplayAlert("Отлично! 🎉",
                        "Все привычки выполнены! Домик стал чище!", "OK");
                }
                else
                {
                    await _houseStateService.UpdateStateAsync();
                }
            }
            catch (Exception ex)
            {
                habitToToggleCompletion.IsCompletedForToday = previousState;
                Debug.WriteLine($"Ошибка обновления выполнения: {ex}");
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить статус выполнения привычки: {ex.Message}", "ОК");
            }
        }

        public async Task RefreshHabitsAsync()
        {
            await LoadHabitsCommand.ExecuteAsync(null);
        }

        private void SortHabits()
        {
            if (Habits == null || Habits.Count <= 1) return;

            var sorted = Habits
                .OrderBy(h => h.IsCompletedForToday)
                .ThenByDescending(h => h.CreatedAt)
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var oldIndex = Habits.IndexOf(sorted[i]);
                if (oldIndex != i && oldIndex != -1)
                {
                    Habits.Move(oldIndex, i);
                }
            }
        }
    }
}