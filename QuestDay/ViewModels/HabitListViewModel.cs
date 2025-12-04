using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using QuestDay.Messages;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.Views;
using System.Collections.ObjectModel;
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
        }

        public async Task InitializeAsync()
        {
            await _habitService.InitializeAsync();
            await LoadHabitsCommand.ExecuteAsync(null);
        }

        public void Receive(NewHabitMessage message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!Habits.Any(h => h.Id == message.Value.Id))
                {
                    Habits.Add(message.Value);
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
                Habits.Clear();
                var habitsFromDb = await _habitService.GetHabitsAsync();
                foreach (var habit in habitsFromDb)
                {
                    habit.IsCompletedForToday = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    Habits.Add(habit);
                }
            }
            catch (Exception ex)
            {
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
                }
                catch (Exception ex)
                {
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
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить статус активности привычки: {ex.Message}", "ОК");
            }
        }

        [RelayCommand]
        private async Task NavigateToAddHabit()
        {
            await Shell.Current.GoToAsync(nameof(AddPage));
        }

    }
}
