using QuestDay.Views;

namespace QuestDay;

public partial class AppShell : Shell
{
    private string currentRoute = nameof(MainPage);

    public string CurrentRoute
    {
        get => currentRoute;
        set
        {
            if (currentRoute == value)
            {
                return;
            }

            currentRoute = value;
            OnPropertyChanged();
        }
    }

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(ListPage), typeof(ListPage));
        Routing.RegisterRoute(nameof(AddPage), typeof(AddPage));
        Routing.RegisterRoute(nameof(SettingPage), typeof(SettingPage));
        Routing.RegisterRoute(nameof(userPage), typeof(userPage));

        Navigated += OnShellNavigated;
        UpdateCurrentRoute(CurrentState?.Location?.OriginalString);
    }

    private async void OnNavClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button?.CommandParameter?.ToString() is string route)
        {
            await Shell.Current.GoToAsync($"//{route}");
        }
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        UpdateCurrentRoute(e.Current?.Location?.OriginalString);
    }

    private void UpdateCurrentRoute(string? location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return;
        }

        var route = location.Split('?', StringSplitOptions.RemoveEmptyEntries)[0]
            .Trim('/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault();

        if (!string.IsNullOrWhiteSpace(route))
        {
            CurrentRoute = route;
        }
    }
}
