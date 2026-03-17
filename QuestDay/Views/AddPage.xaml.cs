using Microsoft.Maui.Controls;
using QuestDay.ViewModels;

namespace QuestDay.Views
{
    public partial class AddPage : ContentPage
    {
        public AddPage(AddHabitViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
         protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AddHabitViewModel viewModel)
            {
                viewModel.Name = string.Empty;
                viewModel.Description = string.Empty;
                viewModel.DaysOfWeekSelection.Reset();
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
