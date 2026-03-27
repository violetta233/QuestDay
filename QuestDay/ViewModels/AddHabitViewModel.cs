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
using QuestDay.Extensions;
using QuestDay.Messages;
using QuestDay.Models;
using QuestDay.Services;

namespace QuestDay.ViewModels
{
    public partial class AddHabitViewModel : ObservableObject
    {
        private readonly IHabitService _habitService;

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

        partial void OnNameChanged(string value)
        {
            ValidateName();
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

        [RelayCommand(CanExecute = nameof(CanSaveHabit))]
        private async Task SaveHabit()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
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

                var habit = new Habit
                {
                    Name = Name,
                    Description = Description,
                    SelectedDays = DaysOfWeekSelection.SelectedDays.ToList(),
                    StartDate = DateTime.Now,
                    IsActive = true
                };

                habit = await _habitService.AddHabitAsync(habit);

                Debug.WriteLine($"Отправка сообщения о новой привычке: {habit.Name}, Id: {habit.Id}");
                WeakReferenceMessenger.Default.Send(new NewHabitMessage(habit));

                // Планируем уведомление для новой привычки
                await ScheduleHabitNotification(habit);

                await Shell.Current.DisplayAlert("Успех", $"Привычка '{habit.Name}' добавлена!", "OK");
                Name = string.Empty;
                Description = string.Empty;
                DaysOfWeekSelection.Reset();

                ValidateName();
                ValidateDays();

                await Shell.Current.GoToAsync("//ListPage");
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
            DaysOfWeekSelection.SelectedDays.Any();

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
                else
                {
                    message = "Заполните все обязательные поля";
                }

                await Shell.Current.DisplayAlert("Заполните форму", message, "OK");
            }
        }

        private async Task ScheduleHabitNotification(Habit habit)
        {
            // Проверим, что напоминания включены в настройках, если нет - выходим
            bool remindersEnabled = Preferences.Default.Get("ReminderEnabled", true);
            if (!remindersEnabled) return;

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            // Получим имя пользователя из настроек приложения
            string userName = Preferences.Default.Get("UserName", "QuestDay");

            // Получим время уведомления из настроек приложения, если не указано - используем 18:00
            string reminderTimeStr = Preferences.Default.Get("ReminderTime", "18:00");
            if (!TimeSpan.TryParse(reminderTimeStr, out TimeSpan reminderTime))
            {
                reminderTime = new TimeSpan(18, 0, 0);
            }

            // Получаем текст напоминания из настроек приложения, если не указано - используем стандартный текст
            string reminderText = Preferences.Default.Get("ReminderText", "Время для вашей привычки!");

            foreach (var day in habit.SelectedDays)
            {
                DateTime notifyTime = GetNextOccurrence(day, reminderTime.Hours, reminderTime.Minutes);

                var request = new NotificationRequest
                {
                    NotificationId = habit.GetNotificationId(day),
                    Title = $"{userName}, {reminderText}",
                    Description = habit.Description,
                    Subtitle = habit.Name,
                    BadgeNumber = 1,

                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime,
                        NotifyRepeatInterval = TimeSpan.FromDays(7)
                    },

                    Image = new NotificationImage
                    {
                        ResourceName = "appicon.png"
                    },

                    ReturningData = "page_to_open=Details&id=" + habit.Id
                };

                await LocalNotificationCenter.Current.Show(request);
            }
        }

        private DateTime GetNextOccurrence(DayOfWeek day, int hour, int minute)
        {
            DateTime start = DateTime.Now.Date.AddHours(hour).AddMinutes(minute);
            if (DateTime.Now >= start) start = start.AddDays(1);

            while (start.DayOfWeek != day)
            {
                start = start.AddDays(1);
            }
            return start;
        }
    }
}