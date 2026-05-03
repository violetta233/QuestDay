using Microsoft.Maui.Controls;
using QuestDay.Models;
using QuestDay.ViewModels;

namespace QuestDay.Views
{
    [QueryProperty(nameof(HabitToEdit), "HabitToEdit")]
    public partial class AddPage : ContentPage
    {
        private readonly AddHabitViewModel _viewModel;
        private Habit _habitToEdit;

        public Habit HabitToEdit
        {
            get => _habitToEdit;
            set
            {
                _habitToEdit = value;
                if (_habitToEdit != null)
                {
                    _viewModel.LoadHabitForEditing(_habitToEdit);
                }
            }
        }

        public AddPage(AddHabitViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (HabitToEdit == null)
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
                await DisplayAlert("Ошибка", $"Не удалось открыть страницу: {ex.Message}", "ОК");
            }
        }
    }
}