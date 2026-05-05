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
    }
}
