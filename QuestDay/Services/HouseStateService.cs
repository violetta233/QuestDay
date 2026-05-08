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
        private System.Timers.Timer? _periodicTimer;
        private DateTime? _incompletionStartTime;

        private const string DirtyLevelKey = "House_DirtyLevel";
        private const string IncompletionStartTimeKey = "House_IncompletionStartTime";

        public event EventHandler<string>? BackgroundChanged;
        public event EventHandler<string>? UserPageBackgroundChanged;
        public event EventHandler<int>? DirtLevelChanged;
        public event EventHandler<bool>? RabbitDirtyStateChanged;

        public HouseStateService(IHabitService habitService)
        {
            _habitService = habitService;

            // Загружаем сохранённое время начала невыполнения
            var savedStartTimeBinary = Preferences.Default.Get(IncompletionStartTimeKey, 0L);
            if (savedStartTimeBinary != 0)
            {
                _incompletionStartTime = DateTime.FromBinary(savedStartTimeBinary);
            }

            StartPeriodicUpdate();
        }

        private void StartPeriodicUpdate()
        {
            _periodicTimer = new System.Timers.Timer(60000); // Каждую минуту
            _periodicTimer.Elapsed += async (s, e) =>
            {
                await UpdateStateAsync();
            };
            _periodicTimer.Start();
            Debug.WriteLine("⏱️ Periodic update timer started (every 60 seconds)");
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

                // Проверяем, все ли активные привычки выполнены
                bool allCompleted = true;
                foreach (var habit in activeHabits)
                {
                    bool isCompleted = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    if (!isCompleted)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);

                if (!allCompleted)
                {
                    // Есть невыполненные привычки
                    if (_incompletionStartTime == null)
                    {
                        _incompletionStartTime = DateTime.UtcNow;
                        Preferences.Default.Set(IncompletionStartTimeKey, _incompletionStartTime.Value.ToBinary());
                        Debug.WriteLine($"🔻 НАЧАЛО периода невыполнения: {_incompletionStartTime}");
                    }

                    var elapsedMinutes = (int)(DateTime.UtcNow - _incompletionStartTime.Value).TotalMinutes;
                    int newDirtyLevel = Math.Min(100, elapsedMinutes);

                    Debug.WriteLine($"⏱️ Прошло минут: {elapsedMinutes}, уровень грязи: {newDirtyLevel}%");

                    if (newDirtyLevel != currentDirtyLevel)
                    {
                        Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);
                        NotifyStateChanged(newDirtyLevel);
                        Debug.WriteLine($"🧹 Уровень грязи: {currentDirtyLevel}% → {newDirtyLevel}%");
                    }
                }
                else
                {
                    // Все привычки выполнены - сбрасываем грязь
                    if (currentDirtyLevel != 0)
                    {
                        _incompletionStartTime = null;
                        Preferences.Default.Remove(IncompletionStartTimeKey);
                        Preferences.Default.Set(DirtyLevelKey, 0);
                        NotifyStateChanged(0);
                        Debug.WriteLine("✨ Все привычки выполнены — домик очищен!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Ошибка в UpdateStateAsync: {ex.Message}");
            }
        }

        public async Task ResetIncompletionStartTime()
        {
            _incompletionStartTime = DateTime.UtcNow;
            Preferences.Default.Set(IncompletionStartTimeKey, _incompletionStartTime.Value.ToBinary());
            Debug.WriteLine($"🔄 Сброс времени начала невыполнения");
            await UpdateStateAsync();
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

                Debug.WriteLine($"📊 NOTIFY: грязь={dirtyLevel}%, isRabbitDirty={isRabbitDirty}");
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
            _incompletionStartTime = null;
            Preferences.Default.Remove(IncompletionStartTimeKey);
            Preferences.Default.Set(DirtyLevelKey, 0);
            NotifyStateChanged(0);
            Debug.WriteLine("🧽 Домик очищен через CleanHouseAsync!");
            await Task.CompletedTask;
        }

        public async Task AddDirtAsync(int amount)
        {
            var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            var newDirtyLevel = Math.Min(100, currentDirtyLevel + amount);
            _incompletionStartTime = DateTime.UtcNow;
            Preferences.Default.Set(IncompletionStartTimeKey, _incompletionStartTime.Value.ToBinary());
            Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);
            NotifyStateChanged(newDirtyLevel);
            Debug.WriteLine($"➕ AddDirtAsync: {currentDirtyLevel} → {newDirtyLevel} (+{amount})");
            await Task.CompletedTask;
        }
    }
}