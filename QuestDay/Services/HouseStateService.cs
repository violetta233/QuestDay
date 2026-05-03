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

        private const string DirtyLevelKey = "House_DirtyLevel";
        private const string IncompletionStartTimeKey = "House_IncompletionStartTime";

        // ⚙️ Для отладки: сброс настроек при запуске (в продакшене поставьте false)
        private const bool ResetPreferencesOnStartup = false;

        public event EventHandler<string>? BackgroundChanged;
        public event EventHandler<int>? DirtLevelChanged;

        public HouseStateService(IHabitService habitService)
        {
            _habitService = habitService;

            if (ResetPreferencesOnStartup)
            {
                Preferences.Default.Remove(DirtyLevelKey);
                Preferences.Default.Remove(IncompletionStartTimeKey);
                Debug.WriteLine("HouseStateService: preferences reset.");
            }
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

                // 🔍 Проверяем, есть ли хоть одна невыполненная привычка
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

                if (hasIncompleteHabits)
                {
                    // 📅 Получаем или устанавливаем время начала периода невыполнения
                    var startTimeBinary = Preferences.Default.Get(IncompletionStartTimeKey, 0L);
                    DateTime incompletionStartTime;

                    if (startTimeBinary == 0 || currentDirtyLevel == 0)
                    {
                        // Первый раз обнаружили невыполнение или только что очистили дом — начинаем отсчёт
                        incompletionStartTime = now;
                        Preferences.Default.Set(IncompletionStartTimeKey, now.ToBinary());
                        Debug.WriteLine($"🔻 Начало периода невыполнения: {incompletionStartTime}");
                    }
                    else
                    {
                        incompletionStartTime = DateTime.FromBinary(startTimeBinary);
                    }

                    // ⏱️ Считаем прошедшие минуты и уровень грязи
                    var elapsedMinutes = (int)(now - incompletionStartTime).TotalMinutes;
                    int newDirtyLevel = Math.Min(100, elapsedMinutes);

                    // 🔄 Обновляем состояние, если изменилось
                    if (newDirtyLevel != currentDirtyLevel)
                    {
                        Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);
                        NotifyStateChanged(newDirtyLevel);
                        Debug.WriteLine($"🧹 Уровень грязи: {currentDirtyLevel}% → {newDirtyLevel}%");
                    }

                    // ▶️ Запускаем таймер для обновления в реальном времени
                    ManageDecayTimer();
                }
                else
                {
                    // ✅ Все привычки выполнены — мгновенно 100% чистоты
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

        private void ManageDecayTimer()
        {
            // Не запускаем таймер, если уже максимум грязи
            var currentLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            if (currentLevel >= 100)
            {
                StopDecayTimer();
                return;
            }

            if (_decayTimer is null)
            {
                _decayTimer = new System.Timers.Timer(60000); // 60 секунд
                _decayTimer.AutoReset = true;
                _decayTimer.Elapsed += async (s, e) =>
                {
                    try
                    {
                        // 🔄 Пересчитываем на основе сохранённого времени начала
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
                Debug.WriteLine("⏱️ Decay timer started: -1% cleanliness per minute");
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
            var backgroundImage = state.GetBackgroundByDirtyLevel();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                BackgroundChanged?.Invoke(this, backgroundImage);
                DirtLevelChanged?.Invoke(this, dirtyLevel);
            });
        }

        public async Task<HouseState> GetCurrentStateAsync()
        {
            var dirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            var state = new HouseState { DirtyLevel = dirtyLevel };
            state.CurrentBackgroundImage = state.GetBackgroundByDirtyLevel();
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