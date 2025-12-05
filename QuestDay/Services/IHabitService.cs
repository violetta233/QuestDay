using System.Collections.Generic;
using System.Threading.Tasks;
using QuestDay.Models;
namespace QuestDay.Services
{
    public interface IHabitService
    {
        Task InitializeAsync();
        Task AddHabitAsync(Habit habit);
        Task<List<Habit>> GetHabitsAsync();
        Task UpdateHabitAsync(Habit habit);
        Task DeleteHabitAsync(Habit habit);
        Task<Habit> GetHabitByIdAsync(int id);


        Task SaveHabitCompletionAsync(int habitId, DateTime date, bool isCompleted);
        Task<bool> GetHabitCompletionStatusAsync(int habitId, DateTime date);
    }
}
