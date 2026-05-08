using Microsoft.Maui.Storage;
using QuestDay.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace QuestDay.Services
{
    public class HouseStateService : IHouseStateService
    {
        private readonly IHabitService _habitService;
        private System.Timers.Timer? _decayTimer;
        private System.Timers.Timer? _periodicTimer;

        private const string DirtyLevelKey = "House_DirtyLevel";
        private const string IncompletionStartTimeKey = "House_IncompletionStartTime";

        private const bool ResetPreferencesOnStartup = false;


        private const double DecayTimerIntervalMilliseconds = 60000;

        public event EventHandler<string>? BackgroundChanged;
        public event EventHandler<string>? UserPageBackgroundChanged;
        public event EventHandler<int>? DirtLevelChanged;
        public event EventHandler<bool>? RabbitDirtyStateChanged;

        public HouseStateService(IHabitService habitService)
        {
            _habitService = habitService;

            if (ResetPreferencesOnStartup)
            {
                Preferences.Default.Remove(DirtyLevelKey);
                Preferences.Default.Remove(IncompletionStartTimeKey);
                Debug.WriteLine("HouseStateService: preferences reset.");
            }

            StartPeriodicUpdate();
        }

        private void StartPeriodicUpdate()
        {
            _periodicTimer = new System.Timers.Timer(10000); // Каждые 10 секунд для теста
            _periodicTimer.Elapsed += async (s, e) =>
            {
                await UpdateStateAsync();
            };
            _periodicTimer.Start();
            Debug.WriteLine("⏱️ Periodic update timer started (every 10 seconds)");
        }

        public async Task UpdateStateAsync()
        {
            Debug.WriteLine($"UpdateStateAsync called at {DateTime.Now}");

            try
            {
                var habits = await _habitService.GetHabitsAsync();
                var activeHabits = habits.Where(h => h.IsActive).ToList();

                if (activeHabits.Count == 0)
                {
                    Debug.WriteLine("UpdateStateAsync: no active habits — skipping.");
                    return;
                }

                bool hasIncompleteHabits = false;
                foreach (var habit in activeHabits)
                {
                    bool isCompleted = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    if (!isCompleted)
                    {
                        hasIncompleteHabits = true;
                        break;
                    }
                }

                var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
                var now = DateTime.UtcNow;

                Debug.WriteLine($"hasIncompleteHabits = {hasIncompleteHabits}, currentDirtyLevel = {currentDirtyLevel}%");

                if (hasIncompleteHabits)
                {
                    var startTimeBinary = Preferences.Default.Get(IncompletionStartTimeKey, 0L);
                    DateTime incompletionStartTime;

                    if (startTimeBinary == 0 || currentDirtyLevel == 0)
                    {
                        incompletionStartTime = now;
                        Preferences.Default.Set(IncompletionStartTimeKey, now.ToBinary());
                        Debug.WriteLine($"🔻 НАЧАЛО периода невыполнения: {incompletionStartTime}");
                    }
                    else
                    {
                        incompletionStartTime = DateTime.FromBinary(startTimeBinary);
                    }

                    var elapsedSeconds = (int)(now - incompletionStartTime).TotalSeconds;
                    int newDirtyLevel = Math.Min(100, elapsedSeconds / 60); // Переводим секунды в минуты

                    // Для теста: 1% за 10 секунд
                    // int newDirtyLevel = Math.Min(100, elapsedSeconds / 10);

                    Debug.WriteLine($"⏱️ Прошло секунд: {elapsedSeconds}, минут: {elapsedSeconds / 60}, новый уровень грязи: {newDirtyLevel}%");

                    if (newDirtyLevel != currentDirtyLevel)
                    {
                        Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);
                        NotifyStateChanged(newDirtyLevel);
                        Debug.WriteLine($"🧹 Уровень грязи: {currentDirtyLevel}% → {newDirtyLevel}%");
                    }

                    ManageDecayTimer();
                }
                else
                {
                    if (currentDirtyLevel != 0)
                    {
                        Preferences.Default.Set(DirtyLevelKey, 0);
                        Preferences.Default.Remove(IncompletionStartTimeKey);
                        NotifyStateChanged(0);
                        Debug.WriteLine("✨ Все привычки выполнены — домик очищен!");
                    }
                    StopDecayTimer();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка в UpdateStateAsync: {ex.Message}");
            }
        }

        public async Task ResetIncompletionStartTime()
        {
            var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            Preferences.Default.Set(IncompletionStartTimeKey, DateTime.UtcNow.ToBinary());
            Debug.WriteLine($"🔄 Сброс времени начала невыполнения (текущий уровень грязи: {currentDirtyLevel}%)");

            // Принудительно запускаем таймер
            StopDecayTimer();
            ManageDecayTimer();

            await UpdateStateAsync();
        }

        private void ManageDecayTimer()
        {
            var currentLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            if (currentLevel >= 100)
            {
                StopDecayTimer();
                return;
            }

            if (_decayTimer is null)
            {
                Debug.WriteLine("🔄 ЗАПУСК decay timer...");
                _decayTimer = new System.Timers.Timer(DecayTimerIntervalMilliseconds);
                _decayTimer.AutoReset = true;
                _decayTimer.Elapsed += async (s, e) =>
                {
                    try
                    {
                        Debug.WriteLine($"⏰ DECAY TIMER СРАБОТАЛ в {DateTime.Now}");

                        var startTimeBinary = Preferences.Default.Get(IncompletionStartTimeKey, 0L);
                        if (startTimeBinary == 0)
                        {
                            StopDecayTimer();
                            return;
                        }

                        var incompletionStartTime = DateTime.FromBinary(startTimeBinary);
                        var now = DateTime.UtcNow;
                        var elapsedMinutes = (int)(now - incompletionStartTime).TotalMinutes;
                        int newDirtyLevel = Math.Min(100, elapsedMinutes);

                        var savedLevel = Preferences.Default.Get(DirtyLevelKey, 0);
                        if (newDirtyLevel != savedLevel)
                        {
                            Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);
                            NotifyStateChanged(newDirtyLevel);
                            Debug.WriteLine($"📈 Уровень грязи увеличен до {newDirtyLevel}%");
                        }

                        if (newDirtyLevel >= 100)
                        {
                            StopDecayTimer();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"⚠️ Ошибка в таймере: {ex.Message}");
                    }
                };
                _decayTimer.Start();
                Debug.WriteLine($"⏱️ Decay timer started: интервал {DecayTimerIntervalMilliseconds}ms");
            }
        }

        private void StopDecayTimer()
        {
            if (_decayTimer is not null)
            {
                _decayTimer.Stop();
                _decayTimer.Dispose();
                _decayTimer = null;
                Debug.WriteLine("🛑 Decay timer stopped");
            }
        }

        private void NotifyStateChanged(int dirtyLevel)
        {
            var state = new HouseState { DirtyLevel = dirtyLevel };
            var mainPageBackground = state.GetMainPageBackground();
            var userPageBackground = state.GetUserPageBackground();
            bool isRabbitDirty = state.IsRabbitDirty;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                BackgroundChanged?.Invoke(this, mainPageBackground);
                UserPageBackgroundChanged?.Invoke(this, userPageBackground);
                DirtLevelChanged?.Invoke(this, dirtyLevel);
                RabbitDirtyStateChanged?.Invoke(this, isRabbitDirty);

                int cleanliness = 100 - dirtyLevel;
                Debug.WriteLine($"📊 NOTIFY: грязь={dirtyLevel}%, чистота={cleanliness}%, isRabbitDirty={isRabbitDirty}");
            });
        }

        public async Task<HouseState> GetCurrentStateAsync()
        {
            var dirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            var state = new HouseState { DirtyLevel = dirtyLevel };
            return await Task.FromResult(state);
        }

        public async Task CleanHouseAsync()
        {
            Preferences.Default.Set(DirtyLevelKey, 0);
            Preferences.Default.Remove(IncompletionStartTimeKey);
            StopDecayTimer();
            NotifyStateChanged(0);
            Debug.WriteLine("🧽 Домик очищен через CleanHouseAsync!");
            await Task.CompletedTask;
        }

        public async Task AddDirtAsync(int amount)
        {
            var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            var newDirtyLevel = Math.Min(100, currentDirtyLevel + amount);
            Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);
            NotifyStateChanged(newDirtyLevel);
            Debug.WriteLine($"➕ AddDirtAsync: {currentDirtyLevel} → {newDirtyLevel} (+{amount})");
            await Task.CompletedTask;
        }
    }
}