using Microsoft.Maui.Controls;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.ViewModels;

namespace QuestDay.Views
{
    [QueryProperty(nameof(HabitId), "HabitId")]
    public partial class AddPage : ContentPage
    {
        private readonly AddHabitViewModel _viewModel;
        private readonly IHabitService _habitService;
        private int _habitId;

        public int HabitId
        {
            get => _habitId;
            set
            {
                _habitId = value;
                if (_habitId > 0)
                {
                    LoadHabitForEditing();
                }
            }
        }

        public AddPage(AddHabitViewModel viewModel, IHabitService habitService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _habitService = habitService;
            BindingContext = _viewModel;
        }

        private async void LoadHabitForEditing()
        {
            try
            {
                var habit = await _habitService.GetHabitByIdAsync(_habitId);
                if (habit != null)
                {
                    _viewModel.LoadHabitForEditing(habit);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить привычку: {ex.Message}", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (HabitId == 0)
            {
                _viewModel.Name = string.Empty;
                _viewModel.Description = string.Empty;
                _viewModel.DaysOfWeekSelection.Reset();
            }
        }

        private async void OnNavClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as ImageButton;
                string route = button.CommandParameter.ToString();
                await Shell.Current.GoToAsync($"//{route}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось выполнить навигацию: {ex.Message}", "ОК");
            }
        }
    }
}