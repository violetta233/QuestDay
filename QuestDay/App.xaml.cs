using Microsoft.Maui.Controls;
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
        public static IAudioPlayer BackgroundMusic { get; private set; }
        private const string SoundEnabledKey = "SoundEnabled";

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;
            LoadBackgroundMusic();
            MainPage = new AppShell();
            Task.Run(async () =>
            {
                await _habitService.InitializeAsync();
            }).Wait();

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
            await _habitService.InitializeAsync();
        }
    }
}