using System.Collections.Generic;
using System.Threading.Tasks;
using QuestDay.Models;
using QuestDay.Services;
namespace QuestDay.Services
{
    public interface IHabitService
    {
        Task AddHabit(Habit habit);
        Task UpdateHabit(Habit habit);
        Task DeleteHabit(Guid id);
        Task<List<Habit>> GetAllHabits();
    }
}