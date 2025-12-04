using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace QuestDay.Models
{
    public class HabitCompletion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int HabitId { get; set; }
        public DateTime CompletionDate { get; set; } 
        public bool IsCompleted { get; set; } 
    }
}

