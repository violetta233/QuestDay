using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using QuestDay.Models;
using QuestDay.ViewModels;

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

            _viewModel.CalendarDaysUpdated += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateCalendarFlex();
                });
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("ListPage OnAppearing - загрузка привычек");
            await _viewModel.InitializeAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateCalendarFlex();
            });
        }

        private void UpdateCalendarFlex()
        {
            CalendarFlexLayout.Children.Clear();

            var days = _viewModel.CalendarDays;
            if (days == null || days.Count == 0) return;

            Debug.WriteLine($"UpdateCalendarFlex: загружено {days.Count} дней, месяц: {_viewModel.CurrentCalendarMonth:yyyy-MM}");

            foreach (var day in days)
            {
                if (day.IsEmpty || day.DayNumber <= 0)
                {
                    var emptyBox = new BoxView
                    {
                        HeightRequest = 38,
                        WidthRequest = 38,
                        BackgroundColor = Colors.Transparent,
                        Margin = new Thickness(2)
                    };
                    CalendarFlexLayout.Children.Add(emptyBox);
                }
                else
                {
                    var border = new Border
                    {
                        HeightRequest = 38,
                        WidthRequest = 38,
                        StrokeThickness = 0,
                        StrokeShape = new RoundRectangle { CornerRadius = 19 },
                        BackgroundColor = day.IsCompleted ? Color.FromArgb("#F68063") : Color.FromArgb("#FDB3A1"),
                        Margin = new Thickness(2)
                    };

                    var label = new Label
                    {
                        Text = day.DayNumber.ToString(),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = day.IsCompleted ? Colors.White : Colors.Black,
                        FontSize = 12,
                        FontFamily = "Montserrat-SemiBold"
                    };

                    if (day.IsToday)
                    {
                        label.FontAttributes = FontAttributes.Bold;
                    }

                    border.Content = label;

                    var dayCopy = day;
                    border.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () =>
                        {
                            await _viewModel.ToggleDayCompletionCommand.ExecuteAsync(dayCopy);
                        })
                    });

                    CalendarFlexLayout.Children.Add(border);
                }
            }
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var habit = button?.CommandParameter as Habit;

                if (habit != null)
                {
                    Debug.WriteLine($"Открытие календаря для привычки: {habit.Name}, Id: {habit.Id}");
                    await _viewModel.ShowCalendarCommand.ExecuteAsync(habit);
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
                await DisplayAlert("Ошибка", $"Не удалось выполнить навигацию: {ex.Message}", "ОК");
            }
        }
    }
}