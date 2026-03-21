using QuestDay.Views;

namespace QuestDay;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(ListPage), typeof(ListPage));
        Routing.RegisterRoute(nameof(AddPage), typeof(AddPage));
        Routing.RegisterRoute(nameof(SettingPage), typeof(SettingPage));
        Routing.RegisterRoute(nameof(userPage), typeof(userPage));
    }

    private async void OnNavClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        if (button?.CommandParameter?.ToString() is string route)
        {
            await Shell.Current.GoToAsync($"//{route}");
        }
    }
}