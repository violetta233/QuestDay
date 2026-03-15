namespace QuestDay;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }
    private async void OnNavClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        string route = button.CommandParameter.ToString();
        await Shell.Current.GoToAsync($"//{route}");
    }
}
