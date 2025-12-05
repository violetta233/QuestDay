using CommunityToolkit.Mvvm.Messaging.Messages;
using QuestDay.Models;
namespace QuestDay.Messages
{
    public class NewHabitMessage : ValueChangedMessage<Habit>
    {
        public NewHabitMessage(Habit value) : base(value)
        {
        }
    }
}
