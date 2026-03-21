using Microsoft.Maui.Controls;
using QuestDay.Services;

namespace QuestDay
{
    public partial class App : Application
    {
        private readonly IHabitService _habitService;

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                await _habitService.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации БД: {ex.Message}");
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}