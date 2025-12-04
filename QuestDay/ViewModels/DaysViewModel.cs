using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuestDay.ViewModels
{
    public partial class DayOption : ObservableObject
    {
        public DayOfWeek Day { get; set; }
        public string Name { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                }
            }
        }

        [RelayCommand]
        private void ToggleSelected()
        {
            IsSelected = !IsSelected;
        }
    }

    public partial class DaysViewModel : ObservableObject
    {
        public ObservableCollection<DayOption> DayOptions { get; } = new ObservableCollection<DayOption>();

        public DaysViewModel()
        {
            InitializeDayOptions();
        }

        private void InitializeDayOptions()
        {
            DayOptions.Clear();

            DayOptions.Add(new DayOption { Day = DayOfWeek.Monday, Name = "Пн" });
            DayOptions.Add(new DayOption { Day = DayOfWeek.Tuesday, Name = "Вт" });
            DayOptions.Add(new DayOption { Day = DayOfWeek.Wednesday, Name = "Ср" });
            DayOptions.Add(new DayOption { Day = DayOfWeek.Thursday, Name = "Чт" });
            DayOptions.Add(new DayOption { Day = DayOfWeek.Friday, Name = "Пт" });
            DayOptions.Add(new DayOption { Day = DayOfWeek.Saturday, Name = "Сб" });
            DayOptions.Add(new DayOption { Day = DayOfWeek.Sunday, Name = "Вс" });

            foreach (var option in DayOptions)
            {
                option.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(DayOption.IsSelected))
                    {
                        OnPropertyChanged(nameof(SelectedDays));
                    }
                };
            }
        }

        public List<DayOfWeek> SelectedDays => DayOptions.Where(d => d.IsSelected).Select(d => d.Day).ToList();

        public void SetSelectedDays(List<DayOfWeek> days)
        {
            foreach (var option in DayOptions)
            {
                option.IsSelected = days.Contains(option.Day);
            }
            OnPropertyChanged(nameof(SelectedDays));
        }

        public void Reset()
        {
            foreach (var option in DayOptions)
            {
                option.IsSelected = false;
            }
            OnPropertyChanged(nameof(SelectedDays));
        }
    }
}
