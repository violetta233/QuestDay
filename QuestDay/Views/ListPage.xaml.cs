using Microsoft.Maui.Controls;
using QuestDay.ViewModels;
using System.Diagnostics;

namespace QuestDay.Views
{
    public partial class ListPage : ContentPage
    {
        private readonly HabitListViewModel _viewModel;

        public ListPage(HabitListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("ListPage OnAppearing - загрузка привычек");
            await _viewModel.InitializeAsync();
        }

        private async void OnNavClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as ImageButton;
                string route = button?.CommandParameter?.ToString();
                if (!string.IsNullOrEmpty(route))
                {
                    await Shell.Current.GoToAsync($"//{route}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось открыть страницу: {ex.Message}", "ОК");
            }
        }
    }
}