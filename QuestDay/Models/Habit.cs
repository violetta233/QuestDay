using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using QuestDay.Models;
using QuestDay.Services;
namespace QuestDay.Models
{
    public class Habit
    {
        internal Guid Id;

        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrEmpty(value) &&
                    !Regex.IsMatch(value, @"^[а-яА-ЯёЁ\s]+$"))
                {
                    throw new ArgumentException("Название должно быть на русском языке");
                }
                _name = value;
            }
        }
        public string Description { get; set; }
        public ObservableCollection<string> SelectedDays { get; set; }
        public DateTime StartDate { get; internal set; }
        private string _name;
        public bool IsActive { get; set; } = true;
    }
}
