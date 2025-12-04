using System;
using System.Collections.Generic;
using SQLite;
using Newtonsoft.Json;

namespace QuestDay.Models
{
    public class Habit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; } = true;

        public string SelectedDaysJson { get; set; }

        [Ignore]
        public List<DayOfWeek> SelectedDays
        {
            get => string.IsNullOrEmpty(SelectedDaysJson)
                ? new List<DayOfWeek>()
                : JsonConvert.DeserializeObject<List<DayOfWeek>>(SelectedDaysJson);
            set => SelectedDaysJson = JsonConvert.SerializeObject(value);
        }

        [Ignore]
        public bool IsCompletedForToday { get; set; } = false;
    }
}


