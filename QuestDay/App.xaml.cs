using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using QuestDay.Services;
using Plugin.Maui.Audio;

namespace QuestDay
{
    public partial class App : Application
    {
        private readonly IHabitService _habitService;
        public static AvatarAppearanceService AvatarAppearance { get; } = new AvatarAppearanceService();
        public static IAudioPlayer? BackgroundMusic { get; private set; }
        private const string SoundEnabledKey = "SoundEnabled";

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;

            Task.Run(async () => await LoadBackgroundMusicAsync()).Wait();

            MainPage = new AppShell();
        }

        private async Task LoadBackgroundMusicAsync()
        {
            try
            {
                string[] possiblePaths =
                {
                    "background_music.mp3",
                    "Audio/background_music.mp3",
                    "Resources/Audio/background_music.mp3",
                    "background_music"
                };

                foreach (var path in possiblePaths)
                {
                    try
                    {
                        var stream = await FileSystem.OpenAppPackageFileAsync(path);
                        if (stream != null)
                        {
                            BackgroundMusic = AudioManager.Current.CreatePlayer(stream);
                            BackgroundMusic.Loop = true;
                            BackgroundMusic.Volume = 0.5;

                            bool isSoundEnabled = Preferences.Default.Get(SoundEnabledKey, true);
                            if (isSoundEnabled)
                            {
                                BackgroundMusic.Play();
                            }

                            System.Diagnostics.Debug.WriteLine($"Музыка загружена! Путь: {path}");
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                System.Diagnostics.Debug.WriteLine("Не удалось найти музыкальный файл");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки музыки: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                await _habitService.InitializeAsync();

                bool isSoundEnabled = Preferences.Default.Get(SoundEnabledKey, true);
                if (isSoundEnabled && BackgroundMusic != null && !BackgroundMusic.IsPlaying)
                {
                    BackgroundMusic.Play();
                    System.Diagnostics.Debug.WriteLine("Музыка запущена");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации БД: {ex.Message}");
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            if (BackgroundMusic?.IsPlaying == true)
            {
                BackgroundMusic.Pause();
                System.Diagnostics.Debug.WriteLine("Музыка на паузе");
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            bool isSoundEnabled = Preferences.Default.Get(SoundEnabledKey, true);
            if (isSoundEnabled && BackgroundMusic != null && !BackgroundMusic.IsPlaying)
            {
                BackgroundMusic.Play();
                System.Diagnostics.Debug.WriteLine("Музыка возобновлена");
            }
        }

        private void OnNotificationTapped(NotificationActionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Уведомление нажато: {e.Request.NotificationId}");
        }
    }
}