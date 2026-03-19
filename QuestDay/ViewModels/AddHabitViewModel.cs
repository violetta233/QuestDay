using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using Plugin.LocalNotification;
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

        public DaysViewModel DaysOfWeekSelection { get; }

        partial void OnNameChanged(string value)
        {
        }

        public AddHabitViewModel(IHabitService habitService)
        {
            _habitService = habitService;
            DaysOfWeekSelection = new DaysViewModel();
            DaysOfWeekSelection.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(DaysViewModel.SelectedDays))
                {
                    SaveHabitCommand.NotifyCanExecuteChanged();
                }
            };
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
                    IsActive = false
                };

                await _habitService.AddHabitAsync(habit);

                WeakReferenceMessenger.Default.Send(new NewHabitMessage(habit));

                // Планируем уведомление для новой привычки
                await ScheduleHabitNotification(habit);

                await Shell.Current.DisplayAlert("Успех", $"Привычка '{habit.Name}' добавлена!", "OK");
                Name = string.Empty;
                Description = string.Empty;
                DaysOfWeekSelection.Reset();

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Произошла ошибка при сохранении: {ex.Message}", "Закрыть");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSaveHabit() => !string.IsNullOrWhiteSpace(Name) && DaysOfWeekSelection.SelectedDays.Any() && !IsBusy;

        private async Task ScheduleHabitNotification(Habit habit)
        {
            // 1. Проверяем/запрашиваем разрешение (обязательно для Android 13+ и iOS)
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            foreach (var day in habit.SelectedDays)
            {   
                DateTime notifyTime = GetNextOccurrence(day, 9, 0);
            
                var request = new NotificationRequest
                {
                    NotificationId = habit.GetNotificationId(day),
                    Title = "QuestDay: Время для вашей привычки!",
                    Description = habit.Description,
                    Subtitle = habit.Name,
                    BadgeNumber = 1,
                    
                    // 2. Установка времени (например, завтра в 9:00)
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime,
                        NotifyRepeatInterval = TimeSpan.FromDays(7) // Повторять еженедельно в выбранный день
                    },

                    // 3. Оформление (Картинка)
                    Image = new NotificationImage
                    {
                        ResourceName = "appicon.png" // Файл должен лежать в Resources/Raw или Platforms/Android/Resources/drawable
                    },

                    // 4. Действие при нажатии
                    ReturningData = "page_to_open=Details&id=" + habit.Id // ID привычки для открытия страницы деталей
                };

                await LocalNotificationCenter.Current.Show(request);        
            }
        }
        
        // Вспомогательный метод для поиска ближайшей даты
        private DateTime GetNextOccurrence(DayOfWeek day, int hour, int minute)
        {
            DateTime start = DateTime.Now.Date.AddHours(hour).AddMinutes(minute);
            
            // Если 9:00 сегодня уже прошло, начинаем поиск со следующего дня
            if (DateTime.Now >= start) start = start.AddDays(1);

            while (start.DayOfWeek != day)
            {
                start = start.AddDays(1);
            }
            return start;
        }    
    }

}
