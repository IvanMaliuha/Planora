using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Planora.ViewModels.Models;


namespace Planora.ViewModels.ViewModels
{
  public class ScheduleViewViewModel : ViewModelBase
  {
    private bool _isWeekView = true;
    private DateTime _currentDate;
    private ScheduleItem _selectedScheduleItem;
    private string _searchText = string.Empty;
    private bool _isLoading;

    public ScheduleViewViewModel()
    {
      CurrentDate = DateTime.Today;

      InitializeSampleData();

      ToggleViewCommand = new RelayCommand(ExecuteToggleView);
      PreviousPeriodCommand = new RelayCommand(ExecutePreviousPeriod);
      NextPeriodCommand = new RelayCommand(ExecuteNextPeriod);
      TodayCommand = new RelayCommand(ExecuteToday);
      ShowDetailsCommand = new RelayCommand(ExecuteShowDetails, CanExecuteShowDetails);

      PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(SearchText) || e.PropertyName == nameof(CurrentDate))
        {
          FilterScheduleItems();
        }
      };
    }

    public ObservableCollection<ScheduleItem> AllScheduleItems { get; private set; }
    public ObservableCollection<ScheduleItem> FilteredScheduleItems { get; private set; }
    public ObservableCollection<ScheduleItem> DailyScheduleItems { get; private set; }

    public bool IsWeekView
    {
      get => _isWeekView;
      set
      {
        SetProperty(ref _isWeekView, value);
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

    public ScheduleItem SelectedScheduleItem
    {
      get => _selectedScheduleItem;
      set
      {
        SetProperty(ref _selectedScheduleItem, value);
        ShowDetailsCommand.RaiseCanExecuteChanged();
      }
    }

    public string SearchText
    {
      get => _searchText;
      set => SetProperty(ref _searchText, value);
    }

    public bool IsLoading
    {
      get => _isLoading;
      set => SetProperty(ref _isLoading, value);
    }

    public string PeriodDisplay { get; private set; } = string.Empty;

    public RelayCommand ToggleViewCommand { get; }
    public RelayCommand PreviousPeriodCommand { get; }
    public RelayCommand NextPeriodCommand { get; }
    public RelayCommand TodayCommand { get; }
    public RelayCommand ShowDetailsCommand { get; }

    private void InitializeSampleData()
    {
      AllScheduleItems = new ObservableCollection<ScheduleItem>
            {
                new ScheduleItem {
                    Day = "Понеділок",
                    Date = GetDateForDay("Понеділок"),
                    Time = "09:00-10:30",
                    Subject = "Математика",
                    Teacher = "Іванов І.І.",
                    Classroom = "101-A",
                    Type = "Лекція",
                    Group = "КН-101"
                },
                new ScheduleItem {
                    Day = "Понеділок",
                    Date = GetDateForDay("Понеділок"),
                    Time = "10:45-12:15",
                    Subject = "Програмування",
                    Teacher = "Петрова М.В.",
                    Classroom = "201-B",
                    Type = "Практика",
                    Group = "КН-101"
                },
                new ScheduleItem {
                    Day = "Вівторок",
                    Date = GetDateForDay("Вівторок"),
                    Time = "09:00-10:30",
                    Subject = "Бази даних",
                    Teacher = "Сидоров О.П.",
                    Classroom = "301-C",
                    Type = "Лекція",
                    Group = "КН-101"
                },
                new ScheduleItem {
                    Day = "Середа",
                    Date = GetDateForDay("Середа"),
                    Time = "13:00-14:30",
                    Subject = "Фізика",
                    Teacher = "Коваленко Т.С.",
                    Classroom = "102-A",
                    Type = "Лекція",
                    Group = "КН-101"
                },
                new ScheduleItem {
                    Day = "Четвер",
                    Date = GetDateForDay("Четвер"),
                    Time = "14:45-16:15",
                    Subject = "Програмування",
                    Teacher = "Петрова М.В.",
                    Classroom = "202-B",
                    Type = "Лабораторна",
                    Group = "КН-101"
                }
            };

      FilteredScheduleItems = new ObservableCollection<ScheduleItem>(AllScheduleItems);
      DailyScheduleItems = new ObservableCollection<ScheduleItem>();
    }


    private void UpdatePeriodDisplay()
    {
      if (IsWeekView)
      {
        var startOfWeek = GetStartOfWeek(CurrentDate);
        var endOfWeek = startOfWeek.AddDays(6);
        PeriodDisplay = $"{startOfWeek:dd MMMM} - {endOfWeek:dd MMMM yyyy}";
      }
      else
      {
        PeriodDisplay = $"{CurrentDate:dd MMMM yyyy}";
      }
      OnPropertyChanged(nameof(PeriodDisplay));
    }

    private void FilterScheduleItems()
    {
      if (AllScheduleItems == null) return;

      var filtered = AllScheduleItems.Where(item =>
      {
        var matchesDate = IsWeekView
                  ? IsInCurrentWeek(item.Date)
                  : item.Date.Date == CurrentDate.Date;

        var matchesSearch = string.IsNullOrEmpty(SearchText) ||
                  item.Subject.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                  item.Teacher.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                  item.Group.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

        return matchesDate && matchesSearch;
      }).OrderBy(item => item.Date).ThenBy(item => item.Time);

      FilteredScheduleItems.Clear();
      foreach (var item in filtered)
      {
        FilteredScheduleItems.Add(item);
      }

      UpdateDailySchedule();
    }

    private void UpdateDailySchedule()
    {
      DailyScheduleItems.Clear();
      var dailyItems = FilteredScheduleItems
          .Where(item => item.Date.Date == CurrentDate.Date)
          .OrderBy(item => item.Time);

      foreach (var item in dailyItems)
      {
        DailyScheduleItems.Add(item);
      }
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

    private DateTime GetDateForDay(string day)
    {
      var today = DateTime.Today;
      var startOfWeek = GetStartOfWeek(today);

      return day switch
      {
        "Понеділок" => startOfWeek,
        "Вівторок" => startOfWeek.AddDays(1),
        "Середа" => startOfWeek.AddDays(2),
        "Четвер" => startOfWeek.AddDays(3),
        "П'ятниця" => startOfWeek.AddDays(4),
        "Субота" => startOfWeek.AddDays(5),
        "Неділя" => startOfWeek.AddDays(6),
        _ => startOfWeek
      };
    }

    private void ExecuteToggleView(object parameter)
    {
      IsWeekView = !IsWeekView;
      FilterScheduleItems();
      Debug.WriteLine($"Переключено на вид: {(IsWeekView ? "Тиждень" : "День")}");
    }

    private void ExecutePreviousPeriod(object parameter)
    {
      CurrentDate = IsWeekView ? CurrentDate.AddDays(-7) : CurrentDate.AddDays(-1);
      Debug.WriteLine($"Попередній період: {PeriodDisplay}");
    }

    private void ExecuteNextPeriod(object parameter)
    {
      CurrentDate = IsWeekView ? CurrentDate.AddDays(7) : CurrentDate.AddDays(1);
      Debug.WriteLine($"Наступний період: {PeriodDisplay}");
    }

    private void ExecuteToday(object parameter)
    {
      CurrentDate = DateTime.Today;
      Debug.WriteLine($"Перехід на сьогодні: {PeriodDisplay}");
    }

    private bool CanExecuteShowDetails(object parameter)
    {
      return SelectedScheduleItem != null;
    }

    private void ExecuteShowDetails(object parameter)
    {
      if (SelectedScheduleItem != null)
      {
        Debug.WriteLine($"Деталі заняття: {SelectedScheduleItem.Subject} - {SelectedScheduleItem.Teacher}");
        // Тут буде відкриття панелі деталей або модального вікна
        // Наприклад: ShowDetailsPanel = true;
      }
    }

    public void RefreshSchedule()
    {
      IsLoading = true;

      // Тут буде завантаження даних з BLL
      // Наприклад: LoadScheduleFromService();

      // Імітація завантаження
      System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
      {
        IsLoading = false;
        FilterScheduleItems();
        Debug.WriteLine("Розклад оновлено");
      });
    }
  }
}