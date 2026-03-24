using Plugin.Maui.Audio;

namespace QuestDay.Services
{
    public interface IBackgroundMusicService
    {
        void StartMusic();
        void StopMusic();
        void ToggleMusic(bool isEnabled);
        bool IsPlaying { get; }
    }

    public class BackgroundMusicService : IBackgroundMusicService
    {
        private IAudioPlayer? _audioPlayer;
        private readonly IAudioManager _audioManager;
        private bool _isMusicEnabled = true;

        public bool IsPlaying => _audioPlayer?.IsPlaying ?? false;

        public BackgroundMusicService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            InitializeMusic();
        }

        private void InitializeMusic()
        {
            try
            {
                // Загружаем музыку из ресурсов
                var stream = FileSystem.OpenAppPackageFileAsync("background_music.mp3").Result;
                _audioPlayer = _audioManager.CreatePlayer(stream);
                _audioPlayer.Loop = true; // Зацикливаем музыку
                _audioPlayer.Volume = 0.5; // Громкость 50%

                // Проверяем сохраненные настройки
                _isMusicEnabled = Preferences.Default.Get("SoundEnabled", true);

                if (_isMusicEnabled)
                {
                    _audioPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации музыки: {ex.Message}");
            }
        }

        public void StartMusic()
        {
            if (_audioPlayer != null && _isMusicEnabled && !_audioPlayer.IsPlaying)
            {
                _audioPlayer.Play();
            }
        }

        public void StopMusic()
        {
            if (_audioPlayer != null && _audioPlayer.IsPlaying)
            {
                _audioPlayer.Stop();
            }
        }

        public void ToggleMusic(bool isEnabled)
        {
            _isMusicEnabled = isEnabled;
            Preferences.Default.Set("SoundEnabled", isEnabled);

            if (isEnabled)
            {
                StartMusic();
            }
            else
            {
                StopMusic();
            }
        }
    }
}