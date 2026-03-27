using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QuestDay.Models;
using SQLite;

namespace QuestDay.Services
{
    public class HabitService : IHabitService
    {
        private SQLiteAsyncConnection? _database;
        private readonly SemaphoreSlim _initializationLock = new(1, 1);

        public HabitService()
        {
        }

        public async Task InitializeAsync()
        {
            await EnsureInitializedAsync();
        }

        public async Task<List<HabitCompletion>> GetHabitCompletionsAsync()
        {
            var database = await EnsureInitializedAsync();
            return await database.Table<HabitCompletion>().ToListAsync();
        }

        public async Task<int> SaveHabitCompletionAsync(HabitCompletion completion)
        {
            var database = await EnsureInitializedAsync();

            if (completion.Id != 0)
            {
                return await database.UpdateAsync(completion);
            }
            else
            {
                return await database.InsertAsync(completion);
            }
        }

        public async Task<Habit> AddHabitAsync(Habit habit)
        {
            var database = await EnsureInitializedAsync();
            Debug.WriteLine($"Сохранение привычки: {habit.Name}");
            await database.InsertAsync(habit);

            var allHabits = await database.Table<Habit>().ToListAsync();
            Debug.WriteLine($"Всего привычек в БД после сохранения: {allHabits.Count}");

            var savedHabit = allHabits.FirstOrDefault(h =>
                h.Name == habit.Name &&
                h.StartDate == habit.StartDate);

            if (savedHabit != null)
            {
                Debug.WriteLine($"Найдена сохраненная привычка: Id={savedHabit.Id}");
                return savedHabit;
            }

            return habit;
        }

        public async Task<List<Habit>> GetHabitsAsync()
        {
            var database = await EnsureInitializedAsync();
            var habits = await database.Table<Habit>().ToListAsync();
            Debug.WriteLine($"GetHabitsAsync: загружено {habits.Count} привычек");
            return habits;
        }

        public async Task UpdateHabitAsync(Habit habit)
        {
            var database = await EnsureInitializedAsync();
            await database.UpdateAsync(habit);
            Debug.WriteLine($"Привычка обновлена: {habit.Name}");
        }

        public async Task DeleteHabitAsync(Habit habit)
        {
            var database = await EnsureInitializedAsync();
            await database.DeleteAsync(habit);
            Debug.WriteLine($"Привычка удалена: {habit.Name}");
        }

        public async Task<Habit> GetHabitByIdAsync(int id)
        {
            var database = await EnsureInitializedAsync();
            return await database.Table<Habit>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task SaveHabitCompletionAsync(int habitId, DateTime date, bool isCompleted)
        {
            var database = await EnsureInitializedAsync();
            DateTime completionDate = date.Date;

            var existingCompletion = await database.Table<HabitCompletion>()
                                                  .Where(c => c.HabitId == habitId && c.CompletionDate == completionDate)
                                                  .FirstOrDefaultAsync();

            if (existingCompletion == null)
            {
                await database.InsertAsync(new HabitCompletion
                {
                    HabitId = habitId,
                    CompletionDate = completionDate,
                    IsCompleted = isCompleted
                });
                Debug.WriteLine($"Создана запись выполнения для привычки {habitId} на {completionDate:dd.MM.yyyy}: {isCompleted}");
            }
            else
            {
                existingCompletion.IsCompleted = isCompleted;
                await database.UpdateAsync(existingCompletion);
                Debug.WriteLine($"Обновлена запись выполнения для привычки {habitId} на {completionDate:dd.MM.yyyy}: {isCompleted}");
            }
        }

        public async Task<bool> GetHabitCompletionStatusAsync(int habitId, DateTime date)
        {
            var database = await EnsureInitializedAsync();
            DateTime completionDate = date.Date;
            var existingCompletion = await database.Table<HabitCompletion>()
                                                  .Where(c => c.HabitId == habitId && c.CompletionDate == completionDate)
                                                  .FirstOrDefaultAsync();
            return existingCompletion?.IsCompleted ?? false;
        }

        private async Task<SQLiteAsyncConnection> EnsureInitializedAsync()
        {
            if (_database is not null)
            {
                return _database;
            }

            await _initializationLock.WaitAsync();
            try
            {
                if (_database is not null)
                {
                    return _database;
                }

                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "QuestDayHabits.db3");
                Debug.WriteLine($"Путь к БД: {dbPath}");
                _database = new SQLiteAsyncConnection(dbPath);
                await _database.CreateTableAsync<HabitCompletion>();
                await _database.CreateTableAsync<Habit>();
                Debug.WriteLine("База данных инициализирована");

                return _database;
            }
            finally
            {
                _initializationLock.Release();
            }
        }
    }
}
