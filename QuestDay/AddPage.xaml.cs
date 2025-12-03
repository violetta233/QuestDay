
using System.Collections.ObjectModel;
using System.ComponentModel;  // Для IValueConverter
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Windows.Input;
namespace QuestDay
{
    public partial class AddPage : ContentPage
    {
        public ObservableCollection<string> SelectedDays { get; set; } = new ObservableCollection<string>();
        public string Name { get; set; }
        public string Description { get; set; }
        public ICommand DayCommand { get; }
        public ICommand SaveHabitCommand { get; }

        public AddPage()
        {
            InitializeComponent();
            BindingContext = this;
            DayCommand = new Command<string>(DaySelected);
            SaveHabitCommand = new Command(SaveHabit);
        }

        private void DaySelected(string day)
        {
            if (SelectedDays.Contains(day))
                SelectedDays.Remove(day);
            else
                SelectedDays.Add(day);
        }

        private void SaveHabit()
        {
            var newHabit = new Models.Habit
            {
                Name = Name,
                Description = Description,
                SelectedDays = SelectedDays
            };

            // Логика сохранения
            Name = "";
            Description = "";
            SelectedDays.Clear();
        }
    }
}
