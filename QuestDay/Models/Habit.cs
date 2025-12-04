using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
namespace QuestDay.Models
{
    public partial class Habit : ObservableObject
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
        private bool _isCompletedForToday = false;
        [Ignore]
        public bool IsCompletedForToday
        {
            get => _isCompletedForToday;
            set => SetProperty(ref _isCompletedForToday, value);
        }
    }
}


