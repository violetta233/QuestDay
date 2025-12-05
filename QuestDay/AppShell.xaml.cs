using QuestDay.Views;
namespace QuestDay
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ListPage), typeof(ListPage));
            Routing.RegisterRoute(nameof(AddPage), typeof(AddPage));
        }
    }
}
