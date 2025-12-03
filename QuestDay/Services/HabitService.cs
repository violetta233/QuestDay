using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QuestDay.Models; // Важно добавить это using
using QuestDay.Services;
namespace QuestDay.Services
{
    public class HabitService : IHabitService
    {
        private List<Habit> _habits = new List<Habit>();

        public async Task AddHabit(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            await Task.Delay(100);
            _habits.Add(habit);
        }

        public async Task UpdateHabit(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            var existingHabit = _habits.FirstOrDefault(h => h.Id == habit.Id);
            if (existingHabit != null)
            {
                existingHabit.Name = habit.Name;
                existingHabit.Description = habit.Description;
            }
        }

        public async Task DeleteHabit(Guid id)
        {
            _habits.RemoveAll(h => h.Id == id);
        }

        public async Task<List<Habit>> GetAllHabits()
        {
            return await Task.FromResult(_habits);
        }
    }
}