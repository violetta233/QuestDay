
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input; // или System.Windows.Input для ICommand


namespace QuestDay.ViewModels
{
    public class DaysViewModel : INotifyPropertyChanged
    {
        private HashSet<string> _selectedDays = new HashSet<string>();

        public HashSet<string> SelectedDays
        {
            get => _selectedDays;
            set
            {
                _selectedDays = value;
                OnPropertyChanged();
            }
        }

        public ICommand DayCommand { get; }

        public DaysViewModel()
        {
            DayCommand = new Command<string>(DaySelected);
        }

        private void DaySelected(string day)
        {
            if (_selectedDays.Contains(day))
            {
                _selectedDays.Remove(day);
            }
            else
            {
                _selectedDays.Add(day);
            }

            OnPropertyChanged(nameof(SelectedDays));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


