using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using QuestDay.Messages;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.Extensions;
using QuestDay.Views;
using Plugin.LocalNotification.AndroidOption;
namespace QuestDay.ViewModels
{
    public partial class AddHabitViewModel : ObservableObject
    {
        private readonly IHabitService _habitService;
        private Habit _editingHabit;
        private string _originalName;
        private string _originalDescription;
        private List<DayOfWeek> _originalSelectedDays;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveHabitCommand))]
        private bool isBusy;

        [ObservableProperty]
        private string nameError;

        [ObservableProperty]
        private string daysError;

        public DaysViewModel DaysOfWeekSelection { get; }

        public bool IsEditingMode => _editingHabit != null;

        partial void OnNameChanged(string value)
        {
            ValidateName();
            SaveHabitCommand.NotifyCanExecuteChanged();
        }

        partial void OnDescriptionChanged(string value)
        {
            SaveHabitCommand.NotifyCanExecuteChanged();
        }

        private void ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                NameError = "Введите название привычки";
            }
            else if (Name.Length < 3)
            {
                NameError = "Название должно содержать минимум 3 символа";
            }
            else
            {
                NameError = null;
            }
        }

        private void ValidateDays()
        {
            if (!DaysOfWeekSelection.SelectedDays.Any())
            {
                DaysError = "Выберите хотя бы один день";
            }
            else
            {
                DaysError = null;
            }
        }

        private bool HasChanges()
        {
            if (_editingHabit == null) return true;

            bool hasChanges = Name != _originalName ||
                             Description != _originalDescription ||
                             !DaysOfWeekSelection.SelectedDays.SequenceEqual(_originalSelectedDays);

            return hasChanges;
        }

        public AddHabitViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            DaysOfWeekSelection = new DaysViewModel();

            DaysOfWeekSelection.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(DaysViewModel.SelectedDays))
                {
                    ValidateDays();
                    SaveHabitCommand.NotifyCanExecuteChanged();
                }
            };

            ValidateName();
            ValidateDays();
        }

        public void LoadHabitForEditing(Habit habit)
        {
            if (habit == null) return;

            _editingHabit = habit;
            _originalName = habit.Name;
            _originalDescription = habit.Description ?? "";
            _originalSelectedDays = habit.SelectedDays.ToList();

            Name = habit.Name;
            Description = habit.Description;
            DaysOfWeekSelection.SetSelectedDays(habit.SelectedDays);
        }

        [RelayCommand(CanExecute = nameof(CanSaveHabit))]
        private async Task SaveHabit()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                await _habitService.InitializeAsync();

                if (string.IsNullOrEmpty(Name))
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, введите название привычки", "Закрыть");
                    return;
                }
                if (!DaysOfWeekSelection.SelectedDays.Any())
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Пожалуйста, выберите хотя бы один день.", "Закрыть");
                    return;
                }

                Habit habit;

                if (_editingHabit != null)
                {
                    _editingHabit.Name = Name;
                    _editingHabit.Description = Description;
                    _editingHabit.SelectedDays = DaysOfWeekSelection.SelectedDays.ToList();

                    await _habitService.UpdateHabitAsync(_editingHabit);
                    habit = _editingHabit;

                    Debug.WriteLine($"Отправка сообщения об обновлении привычки: {habit.Name}, Id: {habit.Id}");
                    WeakReferenceMessenger.Default.Send(new HabitUpdatedMessage(habit));

                    await Shell.Current.GoToAsync("//ListPage");
                }
                else
                {
                    habit = new Habit
                    {
                        Name = Name,
                        Description = Description,
                        SelectedDays = DaysOfWeekSelection.SelectedDays.ToList(),
                        StartDate = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    habit = await _habitService.AddHabitAsync(habit);

                    Debug.WriteLine($"Отправка сообщения о новой привычке: {habit.Name}, Id: {habit.Id}");
                    WeakReferenceMessenger.Default.Send(new NewHabitMessage(habit));
                    await ScheduleHabitNotification(habit);

                    await SuccessPopup.Show($"Привычка '{habit.Name}' добавлена!", navigateToList: true);
                }

                // Очистка формы происходит после успешного сохранения
                Name = string.Empty;
                Description = string.Empty;
                DaysOfWeekSelection.Reset();
                _editingHabit = null;

                ValidateName();
                ValidateDays();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при сохранении: {ex}");
                await Shell.Current.DisplayAlert("Ошибка", $"Произошла ошибка при сохранении: {ex.Message}", "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSaveHabit() =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(Name) &&
            Name.Length >= 3 &&
            DaysOfWeekSelection.SelectedDays.Any() &&
            HasChanges();

        [RelayCommand]
        private async Task ShowValidationTooltip()
        {
            if (!CanSaveHabit())
            {
                string message;
                if (string.IsNullOrWhiteSpace(Name))
                {
                    message = "Введите название привычки";
                }
                else if (Name.Length < 3)
                {
                    message = "Название должно содержать минимум 3 символа";
                }
                else if (!DaysOfWeekSelection.SelectedDays.Any())
                {
                    message = "Выберите хотя бы один день выполнения";
                }
                else if (_editingHabit != null && !HasChanges())
                {
                    message = "Внесите изменения перед сохранением";
                }
                else
                {
                    message = "Заполните все обязательные поля";
                }

                await Shell.Current.DisplayAlert("Заполните форму", message, "OK");
            }
        }

        private async Task ScheduleHabitNotification(Habit habit)
        {
            
            Preferences.Default.Set("ReminderEnabled", true);

            bool remindersEnabled = Preferences.Default.Get("ReminderEnabled", true);
            if (!remindersEnabled) return;

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            string userName = Preferences.Default.Get("UserName", "QuestDay");
            string reminderText = Preferences.Default.Get("ReminderText", "Время для вашей привычки!");
            string reminderTimeStr = Preferences.Default.Get("ReminderTime", "18:00");

            if (!TimeSpan.TryParse(reminderTimeStr, out TimeSpan reminderTime))
            {
                reminderTime = new TimeSpan(18, 0, 0);
            }

            DateTime now = DateTime.Now;

            foreach (var day in habit.SelectedDays)
            {
                
                DateTime notifyTime = DateTime.Today.Add(reminderTime);

                
                if (notifyTime <= now)
                {
                    notifyTime = notifyTime.AddDays(1);
                }

                
                if (notifyTime.DayOfWeek != day)
                {
                    int daysToAdd = ((int)day - (int)notifyTime.DayOfWeek + 7) % 7;
                    notifyTime = notifyTime.AddDays(daysToAdd);
                }

                var request = new NotificationRequest
                {
                    NotificationId = habit.GetNotificationId(day),
                    Title = $"{userName}, {reminderText}",
                    Subtitle = habit.Name,
                    BadgeNumber = 1,

                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime,
                        NotifyRepeatInterval = TimeSpan.FromDays(7)
                    },

                    Image = new NotificationImage { ResourceName = "appicon.png" },
                    ReturningData = "page_to_open=Details&id=" + habit.Id,
                    Android = new AndroidOptions { ChannelId = "questday_channel" }
                };

                await LocalNotificationCenter.Current.Show(request);
            }
        }

        private DateTime GetNextOccurrence(DayOfWeek day, int hour, int minute)
        {
            DateTime now = DateTime.Now;
            DateTime target = now.Date.AddHours(hour).AddMinutes(minute);

            
            if (now >= target)
            {
                target = target.AddDays(1);
            }

            
            int daysAdded = 0;
            while (target.DayOfWeek != day && daysAdded < 7)
            {
                target = target.AddDays(1);
                daysAdded++;
            }

            return target;
        }
    }
}