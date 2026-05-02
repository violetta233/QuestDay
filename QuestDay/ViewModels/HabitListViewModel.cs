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
    public partial class HabitListViewModel : ObservableObject, IRecipient<NewHabitMessage>
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
                    if (selectedHabitForCalendar != null)
                    {
                        Task.Run(async () => await LoadCompletionsForHabit(selectedHabitForCalendar.Id));
                    }
                }
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

        public HabitListViewModel(IHabitService habitService, IHouseStateService houseStateService)
        {
            _habitService = habitService;
            _houseStateService = houseStateService;
            WeakReferenceMessenger.Default.Register<NewHabitMessage>(this);

            Habits.CollectionChanged += (s, e) =>
            {
                Debug.WriteLine($"Коллекция Habits изменилась. Новое количество: {Habits.Count}");
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
            Debug.WriteLine($"Получено сообщение о новой привычке: {message.Value.Name}, Id: {message.Value.Id}");

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!Habits.Any(h => h.Id == message.Value.Id))
                {
                    Habits.Insert(0, message.Value);
                    SortHabits();
                    Debug.WriteLine($"Привычка добавлена в коллекцию. Всего привычек: {Habits.Count}");
                    OnPropertyChanged(nameof(HabitsCount));
                    await _houseStateService.UpdateStateAsync();
                }
                else
                {
                    Debug.WriteLine($"Привычка с Id {message.Value.Id} уже существует в коллекции");
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
                Debug.WriteLine("Начало загрузки привычек");
                Habits.Clear();
                var habitsFromDb = await _habitService.GetHabitsAsync();

                Debug.WriteLine($"Загружено из БД: {habitsFromDb.Count} привычек");

                foreach (var habit in habitsFromDb)
                {
                    Debug.WriteLine($"Добавляем привычку: {habit.Name}, Id: {habit.Id}, IsActive: {habit.IsActive}");
                    habit.IsCompletedForToday = await _habitService.GetHabitCompletionStatusAsync(habit.Id, DateTime.Today);
                    Habits.Add(habit);
                }

                SortHabits();

                Debug.WriteLine($"После добавления в ObservableCollection: {Habits.Count} привычек");

                OnPropertyChanged(nameof(Habits));
                OnPropertyChanged(nameof(HabitsCount));

                await _houseStateService.UpdateStateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки: {ex}");
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
                    Debug.WriteLine($"Привычка '{habitToDelete.Name}' удалена");
                    OnPropertyChanged(nameof(HabitsCount));
                    await _houseStateService.UpdateStateAsync();

                    if (selectedHabitForCalendar?.Id == habitToDelete.Id)
                    {
                        CloseCalendar();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка удаления: {ex}");
                    await Shell.Current.DisplayAlert("Ошибка", $"Не удалось удалить привычку: {ex.Message}", "ОК");
                }
            }
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
                Debug.WriteLine($"Статус активности привычки '{habitToToggle.Name}' изменен на: {habitToToggle.IsActive}");
                await _houseStateService.UpdateStateAsync();
                SortHabits();
            }
            catch (Exception ex)
            {
                habitToToggle.IsActive = previousState;
                Debug.WriteLine($"Ошибка обновления статуса: {ex}");
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
            habitToToggleCompletion.IsCompletedForToday = !previousState;

            try
            {
                await _habitService.SaveHabitCompletionAsync(
                    habitToToggleCompletion.Id,
                    DateTime.Today,
                    habitToToggleCompletion.IsCompletedForToday
                );

                SortHabits();

                Debug.WriteLine($"Статус выполнения привычки '{habitToToggleCompletion.Name}' изменен на: {habitToToggleCompletion.IsCompletedForToday}");

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

                if (allCompleted && activeHabits.Count > 0)
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
                Debug.WriteLine($"Ошибка обновления выполнения: {ex}");
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

            Debug.WriteLine($"Открываем календарь для привычки: {habit.Name}, Id: {habit.Id}");

            SelectedHabitForCalendar = habit;
            await LoadCompletionsForHabit(habit.Id);
            IsCalendarVisible = true;
            OnPropertyChanged(nameof(IsCalendarVisible));
        }

        [RelayCommand]
        private void CloseCalendar()
        {
            Debug.WriteLine("Закрываем календарь");
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
                Debug.WriteLine($"Загружаем выполнения для привычки Id: {habitId}, месяц: {CurrentCalendarMonth:yyyy-MM}");

                var allCompletions = await _habitService.GetCompletionsByHabitIdAsync(habitId);

                var startDate = new DateTime(CurrentCalendarMonth.Year, CurrentCalendarMonth.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var filtered = allCompletions.Where(c => c.CompletionDate >= startDate && c.CompletionDate <= endDate)
                                              .ToList();

                CompletionsForSelectedHabit.Clear();
                foreach (var completion in filtered)
                {
                    CompletionsForSelectedHabit.Add(completion);
                }

                Debug.WriteLine($"Загружено {CompletionsForSelectedHabit.Count} записей о выполнении");

                UpdateCalendarDays();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки истории: {ex}");
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось загрузить историю: {ex.Message}", "ОК");
            }
        }

        [RelayCommand]
        private async Task NextMonth()
        {
            CurrentCalendarMonth = CurrentCalendarMonth.AddMonths(1);
            Debug.WriteLine($"Переход на следующий месяц: {CurrentCalendarMonth:yyyy-MM}");
        }

        [RelayCommand]
        private async Task PreviousMonth()
        {
            CurrentCalendarMonth = CurrentCalendarMonth.AddMonths(-1);
            Debug.WriteLine($"Переход на предыдущий месяц: {CurrentCalendarMonth:yyyy-MM}");
        }

        private bool IsCompletedOnDate(DateTime date)
        {
            return CompletionsForSelectedHabit.Any(c => c.CompletionDate.Date == date.Date && c.IsCompleted);
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
                days.Add(new CalendarDay
                {
                    Date = date,
                    DayNumber = day,
                    IsCompleted = IsCompletedOnDate(date),
                    IsToday = date.Date == DateTime.Today.Date,
                    IsEmpty = false
                });
            }

            CalendarDays = days;
            OnPropertyChanged(nameof(CalendarDays));
            Debug.WriteLine($"Обновлен календарь: {days.Count} дней, из них выполненных: {days.Count(d => d.IsCompleted)}");
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