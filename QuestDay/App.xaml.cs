using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using QuestDay.Services;

namespace QuestDay
{
    public partial class App : Application
    {
        private readonly IHabitService _habitService;
        public static AvatarAppearanceService AvatarAppearance { get; } = new AvatarAppearanceService();

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;

            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;

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

        private void OnNotificationTapped(NotificationActionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Уведомление нажато: {e.Request.NotificationId}");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var data = e.Request.ReturningData;
                if (!string.IsNullOrEmpty(data) && data.Contains("id="))
                {
                }
            });
        }
    }
}