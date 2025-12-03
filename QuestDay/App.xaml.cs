using Microsoft.Maui.Controls;
using QuestDay.Resources;
namespace QuestDay
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppResources.Initialize();
            MainPage = new AppShell(); 
        }
    }
}