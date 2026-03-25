using Microsoft.Maui.Storage;
using QuestDay.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuestDay.Services
{
    public class HouseStateService : IHouseStateService
    {
        private readonly IHabitService _habitService;

        private const string DirtyLevelKey = "House_DirtyLevel";
        private const string LastCompletionDateKey = "House_LastCompletionDate";

        public event EventHandler<string>? BackgroundChanged;
        public event EventHandler<int>? DirtLevelChanged;

        public HouseStateService(IHabitService habitService)
        {
            _habitService = habitService;
        }

        public async Task UpdateStateAsync()
        {
            try
            {
                var habits = await _habitService.GetHabitsAsync();
                var activeHabits = habits.Where(h => h.IsActive).ToList();

                if (activeHabits.Count == 0)
                {
                    await CleanHouseAsync();
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

                var lastCompletionDate = Preferences.Default.Get(LastCompletionDateKey, DateTime.MinValue);
                var today = DateTime.Today;
                var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);

                if (hasIncompleteHabits)
                {
                    if (lastCompletionDate == DateTime.MinValue)
                    {
                        Preferences.Default.Set(LastCompletionDateKey, today);
                        Debug.WriteLine($"Начало невыполнения привычек: {today}");
                    }

                    var daysWithoutCompletion = (today - lastCompletionDate).Days;

                    int newDirtyLevel = 0;

                    if (daysWithoutCompletion >= 2)
                    {
                        newDirtyLevel = 85;
                        Debug.WriteLine($"{daysWithoutCompletion} дня невыполнения - очень грязный уровень");
                    }
                    else if (daysWithoutCompletion >= 1)
                    {
                        newDirtyLevel = 50;
                        Debug.WriteLine($"{daysWithoutCompletion} день невыполнения - грязный уровень");
                    }
                    else
                    {
                        newDirtyLevel = 0;
                    }

                    if (newDirtyLevel != currentDirtyLevel)
                    {
                        Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);

                        var state = new HouseState { DirtyLevel = newDirtyLevel };
                        var backgroundImage = state.GetBackgroundByDirtyLevel();

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            BackgroundChanged?.Invoke(this, backgroundImage);
                            DirtLevelChanged?.Invoke(this, newDirtyLevel);
                        });

                        Debug.WriteLine($"Уровень грязи обновлен: {currentDirtyLevel}% -> {newDirtyLevel}%");
                    }
                }
                else
                {
                    if (lastCompletionDate != DateTime.MinValue)
                    {
                        Preferences.Default.Set(LastCompletionDateKey, DateTime.MinValue);
                        Debug.WriteLine("Привычки выполнены, счетчик невыполнения сброшен");
                    }

                    if (currentDirtyLevel != 0)
                    {
                        await CleanHouseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка проверки состояния домика: {ex.Message}");
            }
        }

        public async Task<HouseState> GetCurrentStateAsync()
        {
            var state = new HouseState
            {
                DirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0)
            };
            state.CurrentBackgroundImage = state.GetBackgroundByDirtyLevel();
            return await Task.FromResult(state);
        }

        public async Task CleanHouseAsync()
        {
            Preferences.Default.Set(DirtyLevelKey, 0);
            Preferences.Default.Set(LastCompletionDateKey, DateTime.MinValue);

            var state = new HouseState { DirtyLevel = 0 };
            var backgroundImage = state.GetBackgroundByDirtyLevel();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                BackgroundChanged?.Invoke(this, backgroundImage);
                DirtLevelChanged?.Invoke(this, 0);
            });

            await Task.CompletedTask;
            Debug.WriteLine("Домик очищен!");
        }

        public async Task AddDirtAsync(int amount)
        {
            var currentDirtyLevel = Preferences.Default.Get(DirtyLevelKey, 0);
            var newDirtyLevel = Math.Min(100, currentDirtyLevel + amount);

            Preferences.Default.Set(DirtyLevelKey, newDirtyLevel);

            var state = new HouseState { DirtyLevel = newDirtyLevel };
            var backgroundImage = state.GetBackgroundByDirtyLevel();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                BackgroundChanged?.Invoke(this, backgroundImage);
                DirtLevelChanged?.Invoke(this, newDirtyLevel);
            });

            await Task.CompletedTask;
        }
    }
}