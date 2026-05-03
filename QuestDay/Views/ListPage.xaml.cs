using Microsoft.Maui.Controls;
using QuestDay.ViewModels;
using QuestDay.Models;
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
            _viewModel.BeautyPopup = BeautyPopupView;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("ListPage OnAppearing - загрузка привычек");
            await _viewModel.InitializeAsync();
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var habit = button?.CommandParameter as Habit;

                if (habit != null)
                {
                    Debug.WriteLine($"Нажата кнопка истории для привычки: {habit.Name}, Id: {habit.Id}");

                    if (_viewModel is HabitListViewModel vm)
                    {
                        await vm.ShowCalendarCommand.ExecuteAsync(habit);
                    }
                }
                else
                {
                    Debug.WriteLine("Ошибка: привычка не найдена");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при открытии календаря: {ex.Message}");
                await DisplayAlert("Ошибка", $"Не удалось открыть календарь: {ex.Message}", "OK");
            }
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