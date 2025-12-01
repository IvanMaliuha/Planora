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
    // Допоміжний клас для групування
    public class ScheduleGroup
    {
        public DateTime Date { get; }
        public string Title { get; }
        public IEnumerable<ScheduleItem> Items { get; }

        public ScheduleGroup(DateTime date, IEnumerable<ScheduleItem> items)
        {
            Date = date;
            Items = items;
            
            var culture = new CultureInfo("uk-UA");
            var dayName = date.ToString("dddd", culture);
            dayName = char.ToUpper(dayName[0]) + dayName.Substring(1);
            
            // Формат заголовку: "Понеділок, 24 листопада"
            Title = $"{dayName}, {date.ToString("d MMMM", culture)}";
        }
    }

    public class ScheduleViewModel : ViewModelBase
    {
        private bool _isWeekView = true;
        private DateTime _currentDate;
        private ScheduleItem _selectedScheduleItem;
        private string _searchText = string.Empty;

        public ScheduleViewModel()
        {
            CurrentDate = DateTime.Today;
            
            // Ініціалізація команд
            ToggleViewCommand = new RelayCommand(ExecuteToggleView);
            PreviousPeriodCommand = new RelayCommand(ExecutePreviousPeriod);
            NextPeriodCommand = new RelayCommand(ExecuteNextPeriod);
            TodayCommand = new RelayCommand(ExecuteToday);
            
            InitializeSampleData(); // Генеруємо дані

            // Реакція на зміни
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SearchText) || e.PropertyName == nameof(CurrentDate))
                {
                    FilterScheduleItems();
                }
            };
            
            UpdatePeriodDisplay();
        }

        // 👇 Змінили тип колекції для UI (тепер це список груп)
        public ObservableCollection<ScheduleGroup> GroupedScheduleItems { get; private set; } = new();
        
        // Зберігаємо всі пари тут
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

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
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
            // Генеруємо дані на широкий діапазон (поточний тиждень +/- 1 тиждень), 
            // щоб навігація працювала і показувала дані
            var startGenDate = GetStartOfWeek(today).AddDays(-7); 

            for (int i = 0; i < 21; i++) // 3 тижні даних
            {
                var date = startGenDate.AddDays(i);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                var dayName = date.ToString("dddd", new CultureInfo("uk-UA"));
                dayName = char.ToUpper(dayName[0]) + dayName.Substring(1);

                // Додаємо пари
                _allScheduleItems.Add(new ScheduleItem {
                    Day = dayName, Date = date, Time = "09:00-10:30",
                    Subject = "Математика (Лекція)", Teacher = "Малюга І.І.",
                    Classroom = "101-A", Type = "Лекція", Group = "КН-101"
                });

                if (i % 2 == 0) // Парні дні
                {
                    _allScheduleItems.Add(new ScheduleItem {
                        Day = dayName, Date = date, Time = "10:45-12:15",
                        Subject = "Програмування (Практика)", Teacher = "Петрова М.В.",
                        Classroom = "201-B", Type = "Практика", Group = "КН-101"
                    });
                }
            }
            FilterScheduleItems();
        }

        private void FilterScheduleItems()
        {
            if (_allScheduleItems == null) return;

            // 1. Фільтруємо плоский список
            var filtered = _allScheduleItems.Where(item =>
            {
                var matchesDate = IsWeekView
                    ? IsInCurrentWeek(item.Date)
                    : item.Date.Date == CurrentDate.Date;

                var matchesSearch = string.IsNullOrEmpty(SearchText) ||
                    item.Subject.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

                return matchesDate && matchesSearch;
            }).OrderBy(item => item.Date).ThenBy(item => item.Time);

            // 2. Групуємо по даті
            var grouped = filtered
                .GroupBy(x => x.Date.Date)
                .Select(g => new ScheduleGroup(g.Key, g.ToList()))
                .ToList();

            // 3. Оновлюємо колекцію для UI
            GroupedScheduleItems.Clear();
            foreach (var group in grouped)
            {
                GroupedScheduleItems.Add(group);
            }
        }

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

        // --- КОМАНДИ НАВІГАЦІЇ ---

        private void ExecuteToggleView(object parameter) => IsWeekView = !IsWeekView;

        private void ExecutePreviousPeriod(object parameter)
        {
            // Якщо тиждень - віднімаємо 7 днів, якщо день - 1 день
            CurrentDate = IsWeekView ? CurrentDate.AddDays(-7) : CurrentDate.AddDays(-1);
        }

        private void ExecuteNextPeriod(object parameter)
        {
            // Аналогічно додаємо
            CurrentDate = IsWeekView ? CurrentDate.AddDays(7) : CurrentDate.AddDays(1);
        }

        private void ExecuteToday(object parameter) => CurrentDate = DateTime.Today;
    }
}