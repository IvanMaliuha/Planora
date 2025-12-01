using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using Planora.ViewModels.Models;
using System.Globalization;
using System.Collections.Generic;

namespace Planora.ViewModels.ViewModels
{
    // Використовуємо той самий клас ScheduleGroup, що і в студентському розкладі
    
    public class TeacherScheduleViewModel : ViewModelBase
    {
        private bool _isWeekView = true;
        private DateTime _currentDate;
        private ScheduleItem _selectedScheduleItem;
        
        public TeacherScheduleViewModel()
        {
            CurrentDate = DateTime.Today;
            
            ToggleViewCommand = new RelayCommand(ExecuteToggleView);
            PreviousPeriodCommand = new RelayCommand(ExecutePreviousPeriod);
            NextPeriodCommand = new RelayCommand(ExecuteNextPeriod);
            TodayCommand = new RelayCommand(ExecuteToday);
            
            InitializeSampleData(); 
            UpdatePeriodDisplay();
        }

        public ObservableCollection<ScheduleGroup> GroupedScheduleItems { get; private set; } = new();
        private List<ScheduleItem> _allScheduleItems = new();

        public bool IsWeekView
        {
            get => _isWeekView;
            set
            {
                SetProperty(ref _isWeekView, value);
                UpdatePeriodDisplay();
                FilterScheduleItems();
            }
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                SetProperty(ref _currentDate, value);
                UpdatePeriodDisplay();
                FilterScheduleItems();
            }
        }

        public string PeriodDisplay { get; private set; } = string.Empty;

        public RelayCommand ToggleViewCommand { get; }
        public RelayCommand PreviousPeriodCommand { get; }
        public RelayCommand NextPeriodCommand { get; }
        public RelayCommand TodayCommand { get; }

        private void InitializeSampleData()
        {
            _allScheduleItems.Clear();
            var today = DateTime.Today;
            var startGenDate = GetStartOfWeek(today).AddDays(-7); 

            // Генеруємо розклад ВИКЛАДАЧА
            for (int i = 0; i < 21; i++) 
            {
                var date = startGenDate.AddDays(i);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                var dayName = date.ToString("dddd", new CultureInfo("uk-UA"));
                dayName = char.ToUpper(dayName[0]) + dayName.Substring(1);

                // Викладач читає "Програмування" для різних груп
                if (i % 2 == 0) // Пн, Ср, Пт
                {
                    _allScheduleItems.Add(new ScheduleItem {
                        Day = dayName, Date = date, Time = "09:00-10:30",
                        Subject = "Програмування (Лекція)", 
                        Teacher = "", // Викладач знає своє ім'я :)
                        Classroom = "101-A", Type = "Лекція", Group = "Потік КН-1" 
                    });
                    
                    _allScheduleItems.Add(new ScheduleItem {
                        Day = dayName, Date = date, Time = "10:45-12:15",
                        Subject = "Програмування (Практика)", 
                        Teacher = "",
                        Classroom = "201-B", Type = "Практика", Group = "КН-101" 
                    });
                }
                else // Вт, Чт
                {
                     _allScheduleItems.Add(new ScheduleItem {
                        Day = dayName, Date = date, Time = "10:45-12:15",
                        Subject = "Програмування (Практика)", 
                        Teacher = "",
                        Classroom = "202-B", Type = "Практика", Group = "КН-102" 
                    });
                }
            }
            FilterScheduleItems();
        }

        private void FilterScheduleItems()
        {
            var filtered = _allScheduleItems.Where(item =>
                IsWeekView ? IsInCurrentWeek(item.Date) : item.Date.Date == CurrentDate.Date
            ).OrderBy(item => item.Date).ThenBy(item => item.Time);

            var grouped = filtered
                .GroupBy(x => x.Date.Date)
                .Select(g => new ScheduleGroup(g.Key, g.ToList()))
                .ToList();

            GroupedScheduleItems.Clear();
            foreach (var group in grouped) GroupedScheduleItems.Add(group);
        }

        // ... (Методи UpdatePeriodDisplay, IsInCurrentWeek, GetStartOfWeek, Команди - такі ж, як у ScheduleViewModel) ...
        // Для скорочення коду я їх пропускаю, але вони повинні бути тут (скопіюйте з ScheduleViewModel.cs)
        
        private void UpdatePeriodDisplay()
        {
            var culture = new CultureInfo("uk-UA");
            if (IsWeekView)
            {
                var startOfWeek = GetStartOfWeek(CurrentDate);
                var endOfWeek = startOfWeek.AddDays(6);
                PeriodDisplay = $"{startOfWeek.ToString("d MMM", culture)} - {endOfWeek.ToString("d MMM yyyy", culture)}";
            }
            else
            {
                PeriodDisplay = $"{CurrentDate.ToString("d MMMM yyyy", culture)}";
            }
            OnPropertyChanged(nameof(PeriodDisplay));
        }

        private bool IsInCurrentWeek(DateTime date)
        {
            var startOfWeek = GetStartOfWeek(CurrentDate);
            var endOfWeek = startOfWeek.AddDays(7);
            return date >= startOfWeek && date < endOfWeek;
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private void ExecuteToggleView(object parameter) => IsWeekView = !IsWeekView;
        private void ExecutePreviousPeriod(object parameter) => CurrentDate = IsWeekView ? CurrentDate.AddDays(-7) : CurrentDate.AddDays(-1);
        private void ExecuteNextPeriod(object parameter) => CurrentDate = IsWeekView ? CurrentDate.AddDays(7) : CurrentDate.AddDays(1);
        private void ExecuteToday(object parameter) => CurrentDate = DateTime.Today;
    }
}