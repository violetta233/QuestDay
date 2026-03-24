using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using QuestDay.Models;
using QuestDay.Resources;
using QuestDay.Services;
using System.Collections.ObjectModel;
using Plugin.Maui.Audio;
namespace QuestDay
{
    public partial class App : Application
    {
        private readonly IHabitService _habitService;
        public static AvatarAppearanceService AvatarAppearance { get; } = new AvatarAppearanceService();
        public static IAudioPlayer BackgroundMusic { get; private set; }
        private const string SoundEnabledKey = "SoundEnabled";

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;
            LoadBackgroundMusic();
            MainPage = new AppShell();
            Task.Run(async () =>
            {
                 await _habitService.InitializeAsync();
            }).Wait();

            MainPage = new AppShell();
        }
        private async void LoadBackgroundMusic()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("Resources/Audio/background.mp3");
                BackgroundMusic = AudioManager.Current.CreatePlayer(stream);
                BackgroundMusic.Loop = true;
                BackgroundMusic.Volume = 0.5;
                bool isSoundEnabled = Preferences.Default.Get(SoundEnabledKey, true);
                if (isSoundEnabled)
                {
                    BackgroundMusic.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
            }
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