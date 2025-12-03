using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using QuestDay.Models;
using QuestDay.Services;

namespace QuestDay
{
    public partial class AddHabitViewModel : ObservableObject
    {
        private readonly IHabitService _habitService;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        public AddHabitViewModel(IHabitService habitService)
        {
            _habitService = habitService;
        }

        [RelayCommand]
        private async Task SaveHabit()
        {
            if (string.IsNullOrEmpty(Name))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, введите название привычки", "Закрыть");
                return;
            }

            var habit = new Habit
            {
                Name = Name,
                Description = Description,
                StartDate = DateTime.Now
            };

            try
            {
                await _habitService.AddHabit(habit);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "Закрыть");
            }
        }
    }
}

