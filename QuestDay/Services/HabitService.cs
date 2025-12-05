using System;
using System.Collections.Generic;
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
                _database = new SQLiteAsyncConnection(dbPath);
                await _database.CreateTableAsync<HabitCompletion>();
                await _database.CreateTableAsync<Habit>();
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
        public async Task AddHabitAsync(Habit habit)
        {
            await _database.InsertAsync(habit);
        }

        public async Task<List<Habit>> GetHabitsAsync()
        {
            return await _database.Table<Habit>().ToListAsync();
        }

        public async Task UpdateHabitAsync(Habit habit)
        {
            await _database.UpdateAsync(habit);
        }

        public async Task DeleteHabitAsync(Habit habit)
        {
            await _database.DeleteAsync(habit);
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
            }
            else
            {
                existingCompletion.IsCompleted = isCompleted;
                await _database.UpdateAsync(existingCompletion);
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
