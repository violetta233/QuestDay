using QuestDay.Models;
using System;

namespace QuestDay.Extensions
{
    public static class HabitExtensions
    {
        public static int GetNotificationId(this Habit habit, DayOfWeek day)
        {
            return habit.Id * 100 + (int)day;
        }
    }
}