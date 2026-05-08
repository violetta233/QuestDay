using Microsoft.Maui.Controls;

namespace QuestDay.Views
{
    public static class SuccessPopup
    {
        public static async Task Show(string message, bool navigateToList = false)
        {
            var tcs = new TaskCompletionSource<bool>();

            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#CC000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 30,
                Padding = new Thickness(25, 20),
                WidthRequest = 280,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            var layout = new VerticalStackLayout { Spacing = 15 };

            layout.Add(new Label
            {
                Text = "✓",
                FontSize = 48,
                TextColor = Color.FromArgb("#A7DFAF"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = "Успех!",
                FontSize = 22,
                FontFamily = "Montserrat-SemiBold",
                TextColor = Color.FromArgb("#4C382F"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = message,
                FontSize = 14,
                TextColor = Color.FromArgb("#666666"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var button = new Button
            {
                Text = "OK",
                BackgroundColor = Color.FromArgb("#A7DFAF"),
                TextColor = Colors.White,
                CornerRadius = 20,
                HeightRequest = 40,
                FontFamily = "Montserrat-SemiBold"
            };

            var popupPage = new ContentPage();

            button.Clicked += async (s, e) =>
            {
                await popupPage.Navigation.PopModalAsync();

                if (navigateToList)
                {
                    await Shell.Current.GoToAsync("//ListPage");
                }

                tcs.SetResult(true);
            };

            layout.Add(button);
            frame.Content = layout;
            grid.Children.Add(frame);
            popupPage.Content = grid;
            popupPage.BackgroundColor = Colors.Transparent;

            await Application.Current.MainPage.Navigation.PushModalAsync(popupPage);
            await tcs.Task;
        }
        public static async Task ShowAllHabitsCompleted(string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#CC000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 30,
                Padding = new Thickness(25, 20),
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            var layout = new VerticalStackLayout { Spacing = 15 };

            // Иконка
            layout.Add(new Label
            {
                Text = "🧹",
                FontSize = 48,
                TextColor = Color.FromArgb("#A7DFAF"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = "Чистота!",
                FontSize = 22,
                FontFamily = "Montserrat-SemiBold",
                TextColor = Color.FromArgb("#4C382F"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = message,
                FontSize = 14,
                TextColor = Color.FromArgb("#666666"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var button = new Button
            {
                Text = "Отлично!",
                BackgroundColor = Color.FromArgb("#A7DFAF"),
                TextColor = Colors.White,
                CornerRadius = 20,
                HeightRequest = 40,
                FontFamily = "Montserrat-SemiBold"
            };

            var popupPage = new ContentPage();

            button.Clicked += async (s, e) =>
            {
                await popupPage.Navigation.PopModalAsync();
                tcs.SetResult(true);
            };

            layout.Add(button);
            frame.Content = layout;
            grid.Children.Add(frame);
            popupPage.Content = grid;
            popupPage.BackgroundColor = Colors.Transparent;

            await Application.Current.MainPage.Navigation.PushModalAsync(popupPage);
            await tcs.Task;
        }
        public static async Task<bool> ShowConfirmation(string title, string message, string yesText = "Да", string noText = "Нет")
        {
            var tcs = new TaskCompletionSource<bool>();

            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#CC000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 30,
                Padding = new Thickness(25, 20),
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            var layout = new VerticalStackLayout { Spacing = 15 };

            layout.Add(new Label
            {
                Text = "❓",
                FontSize = 48,
                TextColor = Color.FromArgb("#A7DFAF"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = title,
                FontSize = 22,
                FontFamily = "Montserrat-SemiBold",
                TextColor = Color.FromArgb("#4C382F"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            layout.Add(new Label
            {
                Text = message,
                FontSize = 14,
                TextColor = Color.FromArgb("#666666"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var buttonGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
        },
                ColumnSpacing = 12
            };

            var yesButton = new Button
            {
                Text = yesText,
                BackgroundColor = Color.FromArgb("#A7DFAF"),
                TextColor = Colors.White,
                CornerRadius = 20,
                HeightRequest = 40,
                FontFamily = "Montserrat-SemiBold"
            };

            var noButton = new Button
            {
                Text = noText,
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                TextColor = Color.FromArgb("#666666"),
                CornerRadius = 20,
                HeightRequest = 40,
                FontFamily = "Montserrat-SemiBold"
            };

            var popupPage = new ContentPage();

            yesButton.Clicked += async (s, e) =>
            {
                await popupPage.Navigation.PopModalAsync();
                tcs.SetResult(true);
            };

            noButton.Clicked += async (s, e) =>
            {
                await popupPage.Navigation.PopModalAsync();
                tcs.SetResult(false);
            };

            buttonGrid.Add(yesButton, 0, 0);
            buttonGrid.Add(noButton, 1, 0);
            layout.Add(buttonGrid);
            frame.Content = layout;
            grid.Children.Add(frame);
            popupPage.Content = grid;
            popupPage.BackgroundColor = Colors.Transparent;

            await Application.Current.MainPage.Navigation.PushModalAsync(popupPage);
            return await tcs.Task;
        }
        public static async Task<bool> ShowDeleteConfirmation(string habitName)
        {
            var tcs = new TaskCompletionSource<bool>();

            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#CC000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 30,
                Padding = 20,
                WidthRequest = 280,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            var layout = new VerticalStackLayout { Spacing = 12 };

            layout.Add(new Label
            {
                Text = "🗑️",
                FontSize = 40,
                TextColor = Color.FromArgb("#FF5252"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = "Удалить привычку?",
                FontSize = 18,
                FontFamily = "Montserrat-SemiBold",
                TextColor = Color.FromArgb("#4C382F"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = $"\"{habitName}\"",
                FontSize = 14,
                TextColor = Color.FromArgb("#666666"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var buttonGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
        },
                ColumnSpacing = 12
            };

            var deleteBtn = new Button
            {
                Text = "Удалить",
                BackgroundColor = Color.FromArgb("#FF5252"),
                TextColor = Colors.White,
                CornerRadius = 20,
                HeightRequest = 44,
                FontFamily = "Montserrat-SemiBold"
            };

            var cancelBtn = new Button
            {
                Text = "Отмена",
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                TextColor = Color.FromArgb("#666666"),
                CornerRadius = 20,
                HeightRequest = 44,
                FontFamily = "Montserrat-SemiBold"
            };

            var popupPage = new ContentPage();
            popupPage.BackgroundColor = Colors.Transparent;

            deleteBtn.Clicked += async (s, e) =>
            {
                await popupPage.Navigation.PopModalAsync();
                tcs.SetResult(true);
            };

            cancelBtn.Clicked += async (s, e) =>
            {
                await popupPage.Navigation.PopModalAsync();
                tcs.SetResult(false);
            };

            buttonGrid.Add(deleteBtn, 0, 0);
            buttonGrid.Add(cancelBtn, 1, 0);
            layout.Add(buttonGrid);
            frame.Content = layout;
            grid.Children.Add(frame);
            popupPage.Content = grid;

            await Application.Current.MainPage.Navigation.PushModalAsync(popupPage);
            return await tcs.Task;
        }
        public static async Task<string> ShowFilterDialog()
        {
            var tcs = new TaskCompletionSource<string>();

            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#CC000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 30,
                Padding = 20,
                WidthRequest = 260,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            var layout = new VerticalStackLayout { Spacing = 12 };

            layout.Add(new Label
            {
                Text = "📋",
                FontSize = 40,
                TextColor = Color.FromArgb("#A7DFAF"),
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = "Фильтр",
                FontSize = 18,
                FontFamily = "Montserrat-SemiBold",
                TextColor = Color.FromArgb("#4C382F"),
                HorizontalOptions = LayoutOptions.Center
            });

            var allBtn = new Button
            {
                Text = "Все привычки",
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                TextColor = Color.FromArgb("#333333"),
                CornerRadius = 20,
                HeightRequest = 44
            };

            var todayBtn = new Button
            {
                Text = "Задачи на сегодня",
                BackgroundColor = Color.FromArgb("#E0E0E0"),
                TextColor = Color.FromArgb("#333333"),
                CornerRadius = 20,
                HeightRequest = 44
            };

            var popup = new ContentPage();
            popup.BackgroundColor = Colors.Transparent;

            allBtn.Clicked += async (s, e) =>
            {
                await popup.Navigation.PopModalAsync();
                tcs.SetResult("Все привычки");
            };

            todayBtn.Clicked += async (s, e) =>
            {
                await popup.Navigation.PopModalAsync();
                tcs.SetResult("Задачи на сегодня");
            };

            layout.Add(allBtn);
            layout.Add(todayBtn);
            frame.Content = layout;
            grid.Children.Add(frame);
            popup.Content = grid;

            await Application.Current.MainPage.Navigation.PushModalAsync(popup);
            return await tcs.Task;
        }
        public static async Task ShowInfoPopup(string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            var grid = new Grid
            {
                BackgroundColor = Color.FromArgb("#CC000000"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 30,
                Padding = 20,
                WidthRequest = 260,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HasShadow = true
            };

            var layout = new VerticalStackLayout
            {
                Spacing = 12
            };

            layout.Add(new Label
            {
                Text = "⏳",
                FontSize = 40,
                HorizontalOptions = LayoutOptions.Center
            });

            layout.Add(new Label
            {
                Text = message,
                FontSize = 16,
                FontFamily = "Montserrat-SemiBold",
                TextColor = Color.FromArgb("#4C382F"),
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center
            });

            var okBtn = new Button
            {
                Text = "ОК",
                BackgroundColor = Color.FromArgb("#A7DFAF"),
                TextColor = Colors.White,
                CornerRadius = 20,
                HeightRequest = 44,
                FontFamily = "Montserrat-SemiBold"
            };

            var popup = new ContentPage();
            popup.BackgroundColor = Colors.Transparent;

            okBtn.Clicked += async (s, e) =>
            {
                await popup.Navigation.PopModalAsync();
                tcs.SetResult(true);
            };

            layout.Add(okBtn);

            frame.Content = layout;
            grid.Children.Add(frame);
            popup.Content = grid;

            await Application.Current.MainPage.Navigation.PushModalAsync(popup);

            await tcs.Task;
        }
    }
}