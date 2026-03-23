using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestDay.Models;
using SQLite;

namespace QuestDay.Services
{
    public class HabitService : IHabitService
    {
        private SQLiteAsyncConnection _database;

        public HabitService()
        {
        }

        public async Task InitializeAsync()
        {
            if (_database is not null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "QuestDayHabits.db3");
            Debug.WriteLine($"Путь к БД: {dbPath}");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<HabitCompletion>();
            await _database.CreateTableAsync<Habit>();
            Debug.WriteLine("База данных инициализирована");
        }

        public Task<List<HabitCompletion>> GetHabitCompletionsAsync()
        {
            return _database.Table<HabitCompletion>().ToListAsync();
        }

        public Task<int> SaveHabitCompletionAsync(HabitCompletion completion)
        {
            if (completion.Id != 0)
            {
                return _database.UpdateAsync(completion);
            }
            else
            {
                return _database.InsertAsync(completion);
            }
        }

        public async Task<Habit> AddHabitAsync(Habit habit)
        {
            Debug.WriteLine($"Сохранение привычки: {habit.Name}");
            await _database.InsertAsync(habit);

            var allHabits = await _database.Table<Habit>().ToListAsync();
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
            var habits = await _database.Table<Habit>().ToListAsync();
            Debug.WriteLine($"GetHabitsAsync: загружено {habits.Count} привычек");
            return habits;
        }

        public async Task UpdateHabitAsync(Habit habit)
        {
            await _database.UpdateAsync(habit);
            Debug.WriteLine($"Привычка обновлена: {habit.Name}");
        }

        public async Task DeleteHabitAsync(Habit habit)
        {
            await _database.DeleteAsync(habit);
            Debug.WriteLine($"Привычка удалена: {habit.Name}");
        }

        public async Task<Habit> GetHabitByIdAsync(int id)
        {
            return await _database.Table<Habit>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task SaveHabitCompletionAsync(int habitId, DateTime date, bool isCompleted)
        {
            DateTime completionDate = date.Date;

            var existingCompletion = await _database.Table<HabitCompletion>()
                                                  .Where(c => c.HabitId == habitId && c.CompletionDate == completionDate)
                                                  .FirstOrDefaultAsync();

            if (existingCompletion == null)
            {
                await _database.InsertAsync(new HabitCompletion
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
                await _database.UpdateAsync(existingCompletion);
                Debug.WriteLine($"Обновлена запись выполнения для привычки {habitId} на {completionDate:dd.MM.yyyy}: {isCompleted}");
            }
        }

        public async Task<bool> GetHabitCompletionStatusAsync(int habitId, DateTime date)
        {
            DateTime completionDate = date.Date;
            var existingCompletion = await _database.Table<HabitCompletion>()
                                                  .Where(c => c.HabitId == habitId && c.CompletionDate == completionDate)
                                                  .FirstOrDefaultAsync();
            return existingCompletion?.IsCompleted ?? false;
        }
    }
}