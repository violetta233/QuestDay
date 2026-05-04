using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using QuestDay.Messages;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace QuestDay.ViewModels
{
    public partial class HabitListViewModel : ObservableObject, IRecipient<NewHabitMessage>, IRecipient<HabitUpdatedMessage>
    {
        private readonly IHabitService _habitService;
        private readonly IHouseStateService _houseStateService;

        public ObservableCollection<Habit> Habits { get; } = new ObservableCollection<Habit>();

        [ObservableProperty]
        private bool isBusy;

        public int HabitsCount => Habits.Count;

        [ObservableProperty]
        private Habit selectedHabitForCalendar;

        [ObservableProperty]
        private bool isCalendarVisible;

        [ObservableProperty]
        private ObservableCollection<HabitCompletion> completionsForSelectedHabit = new();

        private DateTime currentCalendarMonth = DateTime.Today;

        public DateTime CurrentCalendarMonth
        {
            get => currentCalendarMonth;
            set
            {
                if (SetProperty(ref currentCalendarMonth, value))
                {
                    OnPropertyChanged(nameof(CurrentCalendarMonth));
                    OnPropertyChanged(nameof(CurrentCalendarMonthRussian));
                    OnPropertyChanged(nameof(CompletionsCountText));
                    if (selectedHabitForCalendar != null)
                    {
                        Task.Run(async () => await LoadCompletionsForHabit(selectedHabitForCalendar.Id));
                    }
                }
            }
        }

        public string CompletionsCountText
        {
            get
            {
                var startDate = new DateTime(CurrentCalendarMonth.Year, CurrentCalendarMonth.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var count = CompletionsForSelectedHabit
                    .Where(c => c.CompletionDate >= startDate && c.CompletionDate <= endDate && c.IsCompleted)
                    .Count();

                if (count == 0) return "0 дней";
                if (count == 1) return "1 день";
                if (count >= 2 && count <= 4) return $"{count} дня";
                return $"{count} дней";
            }
        }

        public string CurrentCalendarMonthRussian
        {
            get
            {
                var culture = new CultureInfo("ru-RU");
                return currentCalendarMonth.ToString("MMMM yyyy", culture);
            }
        }

        [ObservableProperty]
        private ObservableCollection<CalendarDay> calendarDays = new();

        public BeautyPopup BeautyPopup { get; set; }

        public HabitListViewModel(IHabitService habitService, IHouseStateService houseStateService)
        {
            _habitService = habitService;
            _houseStateService = houseStateService;
            WeakReferenceMessenger.Default.Register<NewHabitMessage>(this);
            WeakReferenceMessenger.Default.Register<HabitUpdatedMessage>(this);

            Habits.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HabitsCount));
            };
        }

        public async Task InitializeAsync()
        {
            await _habitService.InitializeAsync();
            await LoadHabitsCommand.ExecuteAsync(null);
        }

        public void Receive(NewHabitMessage message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!Habits.Any(h => h.Id == message.Value.Id))
                {
                    Habits.Insert(0, message.Value);
                    SortHabits();
                    OnPropertyChanged(nameof(HabitsCount));
                    await _houseStateService.UpdateStateAsync();
                }
            });
        }

        public void Receive(HabitUpdatedMessage message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var index = Habits.IndexOf(message.Value);
                if (index != -1)
                {
                    Habits[index] = message.Value;
                    SortHabits();
                    OnPropertyChanged(nameof(Habits));
                    OnPropertyChanged(nameof(HabitsCount));
                }
            });
        }

        [RelayCommand(CanExecute = nameof(CanLoadHabits))]
        private async Task LoadHabits()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                Habits.Clear();
                var habitsFromDb = await _habitService.GetHabitsAsync();

                foreach (var habit in habitsFromDb)
                {
                    habit.IsCompletedForToday = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    Habits.Add(habit);
                }

                SortHabits();

                OnPropertyChanged(nameof(Habits));
                OnPropertyChanged(nameof(HabitsCount));

                await _houseStateService.UpdateStateAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить привычки: {ex.Message}", "ОК");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLoadHabits() => !IsBusy;

        [RelayCommand]
        private async Task DeleteHabit(Habit habitToDelete)
        {
            if (habitToDelete == null) return;

            bool confirm = await Shell.Current.DisplayAlert("Удалить привычку", $"Вы уверены, что хотите удалить '{habitToDelete.Name}'?", "Да", "Нет");
            if (confirm)
            {
                try
                {
                    await _habitService.DeleteHabitAsync(habitToDelete);
                    Habits.Remove(habitToDelete);
                    OnPropertyChanged(nameof(HabitsCount));
                    await _houseStateService.UpdateStateAsync();

                    if (selectedHabitForCalendar?.Id == habitToDelete.Id)
                    {
                        CloseCalendar();
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Ошибка", $"Не удалось удалить привычку: {ex.Message}", "ОК");
                }
            }
        }

        [RelayCommand]
        private async Task EditHabit(Habit habit)
        {
            if (habit == null) return;

            var editHabit = new Habit
            {
                Id = habit.Id,
                Name = habit.Name,
                Description = habit.Description,
                SelectedDays = habit.SelectedDays.ToList(),
                StartDate = habit.StartDate,
                IsActive = habit.IsActive,
                SelectedDaysJson = habit.SelectedDaysJson
            };

            var navigationParameter = new Dictionary<string, object>
            {
                { "HabitToEdit", editHabit }
            };

            await Shell.Current.GoToAsync(nameof(AddPage), navigationParameter);
        }

        [RelayCommand]
        private async Task ToggleHabitIsActiveStatus(Habit habitToToggle)
        {
            if (habitToToggle == null) return;

            bool previousState = habitToToggle.IsActive;
            habitToToggle.IsActive = !previousState;

            try
            {
                await _habitService.UpdateHabitAsync(habitToToggle);
                await _houseStateService.UpdateStateAsync();
                SortHabits();
            }
            catch (Exception ex)
            {
                habitToToggle.IsActive = previousState;
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить статус активности привычки: {ex.Message}", "ОК");
            }
        }

        [RelayCommand]
        private async Task NavigateToAddHabit()
        {
            await Shell.Current.GoToAsync(nameof(AddPage));
        }

        [RelayCommand]
        private async Task ToggleHabitCompletion(Habit habitToToggleCompletion)
        {
            if (habitToToggleCompletion == null) return;

            bool previousState = habitToToggleCompletion.IsCompletedForToday;
            bool newState = !previousState;
            habitToToggleCompletion.IsCompletedForToday = newState;

            try
            {
                await _habitService.SaveHabitCompletionAsync(
                    habitToToggleCompletion.Id,
                    DateTime.Today,
                    newState
                );

                SortHabits();

                var allHabits = await _habitService.GetHabitsAsync();
                var activeHabits = allHabits.Where(h => h.IsActive).ToList();

                bool allCompleted = true;
                foreach (var habit in activeHabits)
                {
                    bool isCompleted = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    if (!isCompleted)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                if (allCompleted && activeHabits.Count > 0 && newState == true)
                {
                    await _houseStateService.CleanHouseAsync();
                    await Shell.Current.DisplayAlert("Отлично! 🎉",
                        "Все привычки выполнены! Домик стал чище!", "OK");
                }
                else
                {
                    await _houseStateService.UpdateStateAsync();
                }
            }
            catch (Exception ex)
            {
                habitToToggleCompletion.IsCompletedForToday = previousState;
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить статус выполнения привычки: {ex.Message}", "ОК");
            }
        }

        public async Task RefreshHabitsAsync()
        {
            await LoadHabitsCommand.ExecuteAsync(null);
        }

        private void SortHabits()
        {
            if (Habits == null || Habits.Count <= 1) return;

            var sorted = Habits
                .OrderBy(h => h.IsCompletedForToday)
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var oldIndex = Habits.IndexOf(sorted[i]);
                if (oldIndex != i && oldIndex != -1)
                {
                    Habits.Move(oldIndex, i);
                }
            }
        }

        [RelayCommand]
        private async Task ShowCalendar(Habit habit)
        {
            if (habit == null) return;
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                SelectedHabitForCalendar = habit;
                await LoadCompletionsForHabit(habit.Id);
                IsCalendarVisible = true;
                OnPropertyChanged(nameof(IsCalendarVisible));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось открыть календарь: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void CloseCalendar()
        {
            IsCalendarVisible = false;
            SelectedHabitForCalendar = null;
            CompletionsForSelectedHabit.Clear();
            CalendarDays.Clear();
            OnPropertyChanged(nameof(IsCalendarVisible));
        }

        private async Task LoadCompletionsForHabit(int habitId)
        {
            try
            {
                var allCompletions = await _habitService.GetCompletionsByHabitIdAsync(habitId);

                CompletionsForSelectedHabit.Clear();
                foreach (var completion in allCompletions)
                {
                    CompletionsForSelectedHabit.Add(completion);
                }

                OnPropertyChanged(nameof(CompletionsCountText));
                UpdateCalendarDays();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить историю: {ex.Message}", "ОК");
            }
        }

        [RelayCommand]
        private async Task NextMonth()
        {
            CurrentCalendarMonth = CurrentCalendarMonth.AddMonths(1);
        }

        [RelayCommand]
        private async Task PreviousMonth()
        {
            CurrentCalendarMonth = CurrentCalendarMonth.AddMonths(-1);
        }

        [RelayCommand]
        private async Task ToggleDayCompletion(CalendarDay day)
        {
            if (day == null || day.IsEmpty || day.DayNumber <= 0) return;
            if (SelectedHabitForCalendar == null) return;
            if (BeautyPopup == null) return;

            try
            {
                var date = day.Date;
                var newStatus = !day.IsCompleted;
                var statusText = newStatus ? "выполненный" : "невыполненный";

                var confirm = await BeautyPopup.ShowAsync(
                    "Подтверждение",
                    $"Отметить {date:dd.MM.yyyy} как {statusText}?",
                    "Да",
                    "Нет",
                    newStatus ? "✅" : "❌"
                );

                if (!confirm) return;

                var isCompleted = newStatus;

                await _habitService.SaveHabitCompletionAsync(SelectedHabitForCalendar.Id, date, isCompleted);
                day.IsCompleted = isCompleted;

                var existing = CompletionsForSelectedHabit.FirstOrDefault(c => c.CompletionDate.Date == date.Date);
                if (existing != null)
                {
                    if (isCompleted) existing.IsCompleted = true;
                    else CompletionsForSelectedHabit.Remove(existing);
                }
                else if (isCompleted)
                {
                    CompletionsForSelectedHabit.Add(new HabitCompletion
                    {
                        HabitId = SelectedHabitForCalendar.Id,
                        CompletionDate = date,
                        IsCompleted = true
                    });
                }

                OnPropertyChanged(nameof(CompletionsCountText));
                UpdateCalendarDays();

                if (date.Date == DateTime.Today.Date)
                {
                    var habit = Habits.FirstOrDefault(h => h.Id == SelectedHabitForCalendar.Id);
                    if (habit != null)
                    {
                        habit.IsCompletedForToday = isCompleted;
                        SortHabits();
                        OnPropertyChanged(nameof(Habits));
                    }
                }

                await _houseStateService.UpdateStateAsync();

                await BeautyPopup.ShowAsync(
                    isCompleted ? "Выполнено!" : "Статус изменён",
                    isCompleted ? $"✅ За {date:dd.MM.yyyy} отмечено!" : $"❌ За {date:dd.MM.yyyy} отметка снята",
                    "OK",
                    "",
                    isCompleted ? "✅" : "❌"
                );
            }
            catch (Exception ex)
            {
                await BeautyPopup.ShowAsync("Ошибка", ex.Message, "OK", "", "⚠️");
            }
        }

        private void UpdateCalendarDays()
        {
            var days = new ObservableCollection<CalendarDay>();

            var firstDayOfMonth = new DateTime(CurrentCalendarMonth.Year, CurrentCalendarMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(CurrentCalendarMonth.Year, CurrentCalendarMonth.Month);

            int startOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            for (int i = 0; i < startOffset; i++)
            {
                days.Add(new CalendarDay { IsEmpty = true, DayNumber = -1 });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(CurrentCalendarMonth.Year, CurrentCalendarMonth.Month, day);
                var completion = CompletionsForSelectedHabit.FirstOrDefault(c => c.CompletionDate.Date == date.Date);
                var isCompleted = completion != null && completion.IsCompleted;

                days.Add(new CalendarDay
                {
                    Date = date,
                    DayNumber = day,
                    IsCompleted = isCompleted,
                    IsToday = date.Date == DateTime.Today.Date,
                    IsEmpty = false
                });
            }

            CalendarDays = days;
            OnPropertyChanged(nameof(CalendarDays));
            OnPropertyChanged(nameof(CompletionsCountText));
        }
    }

    public class CalendarDay : ObservableObject
    {
        private bool _isCompleted;
        private bool _isToday;
        private bool _isEmpty;
        private int _dayNumber;
        private DateTime _date;

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public int DayNumber
        {
            get => _dayNumber;
            set => SetProperty(ref _dayNumber, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public bool IsToday
        {
            get => _isToday;
            set => SetProperty(ref _isToday, value);
        }

        public bool IsEmpty
        {
            get => _isEmpty;
            set => SetProperty(ref _isEmpty, value);
        }
    }
}