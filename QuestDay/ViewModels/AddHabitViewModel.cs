using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using QuestDay.Messages;
using QuestDay.Models;
using QuestDay.Services;

namespace QuestDay.ViewModels
{
    public partial class AddHabitViewModel : ObservableObject
    {
        private readonly IHabitService _habitService;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveHabitCommand))]
        private bool isBusy;

        public DaysViewModel DaysOfWeekSelection { get; }

        partial void OnNameChanged(string value)
        {
        }

        public AddHabitViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            DaysOfWeekSelection = new DaysViewModel();
            DaysOfWeekSelection.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(DaysViewModel.SelectedDays))
                {
                    SaveHabitCommand.NotifyCanExecuteChanged();
                }
            };
        }

        [RelayCommand(CanExecute = nameof(CanSaveHabit))]
        private async Task SaveHabit()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, введите название привычки", "Закрыть");
                    return;
                }
                if (!DaysOfWeekSelection.SelectedDays.Any())
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, выберите хотя бы один день.", "Закрыть");
                    return;
                }

                var habit = new Habit
                {
                    Name = Name,
                    Description = Description,
                    SelectedDays = DaysOfWeekSelection.SelectedDays.ToList(),
                    StartDate = DateTime.Now,
                    IsActive = false
                };

                await _habitService.AddHabitAsync(habit);

                WeakReferenceMessenger.Default.Send(new NewHabitMessage(habit));

                await Shell.Current.DisplayAlert("Успех", $"Привычка '{habit.Name}' добавлена!", "OK");
                Name = string.Empty;
                Description = string.Empty;
                DaysOfWeekSelection.Reset();

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Произошла ошибка при сохранении: {ex.Message}", "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSaveHabit() => !string.IsNullOrWhiteSpace(Name) && DaysOfWeekSelection.SelectedDays.Any() && !IsBusy;
    }
}
