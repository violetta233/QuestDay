using QuestDay.Models;

namespace QuestDay.Messages
{
    public class HabitUpdatedMessage
    {
        public Habit Value { get; set; }

        public HabitUpdatedMessage(Habit habit)
        {
            Value = habit;
        }
    }
}