using Microsoft.Maui.Controls;
using QuestDay.Services;
using QuestDay.Models;
using QuestDay.Resources;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace QuestDay
{
public partial class App : Application
{
    private readonly IHabitService _habitService;
    public static AvatarAppearanceService AvatarAppearance { get; private set; } = null!;

    public App(IHabitService habitService, AvatarAppearanceService avatarAppearance)
    {
        InitializeComponent();
        _habitService = habitService;
        AvatarAppearance = avatarAppearance;

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
            if (e.IsTapped)
            {
                // Логика перехода на нужную страницу на основе e.Request.ReturningData
                // Shell.Current.GoToAsync("///DetailsPage");
            }
        }        
    }
}
