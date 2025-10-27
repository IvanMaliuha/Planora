using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace Planora.ViewModels.ViewModels
{
  public class ClassroomSearchViewModel : ViewModelBase
  {
    private DateTime _startTime = DateTime.Now;
    private DateTime _endTime = DateTime.Now.AddHours(2);
    private string _selectedBuilding = "Всі";
    private bool _isSearching;

    public ClassroomSearchViewModel()
    {
      SearchResults = new ObservableCollection<ClassroomItem>();
      Buildings = new ObservableCollection<string> { "Всі", "A", "B", "C" };
      ClassroomTypes = new ObservableCollection<string> { "Всі", "Лекційна", "Практична", "Лабораторна" };

      AllClassrooms = new ObservableCollection<ClassroomItem>
            {
                new ClassroomItem { Number = "101", Building = "A", Type = "Лекційна", Capacity = 50, HasComputers = true, HasProjector = true },
                new ClassroomItem { Number = "102", Building = "A", Type = "Лекційна", Capacity = 60, HasComputers = false, HasProjector = true },
                new ClassroomItem { Number = "201", Building = "B", Type = "Лабораторна", Capacity = 25, HasComputers = true, HasProjector = false },
                new ClassroomItem { Number = "202", Building = "B", Type = "Лабораторна", Capacity = 30, HasComputers = true, HasProjector = true },
                new ClassroomItem { Number = "301", Building = "C", Type = "Практична", Capacity = 30, HasComputers = false, HasProjector = true },
                new ClassroomItem { Number = "302", Building = "C", Type = "Практична", Capacity = 40, HasComputers = true, HasProjector = true }
            };

      SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteSearch);
      UseMyScheduleCommand = new RelayCommand(ExecuteUseMySchedule);
    }

    public DateTime StartTime
    {
      get => _startTime;
      set => SetProperty(ref _startTime, value);
    }

    public DateTime EndTime
    {
      get => _endTime;
      set => SetProperty(ref _endTime, value);
    }

    private bool _hasComputers;
    private bool _hasProjector;

    public bool HasComputers
    {
      get => _hasComputers;
      set => SetProperty(ref _hasComputers, value);
    }

    public bool HasProjector
    {
      get => _hasProjector;
      set => SetProperty(ref _hasProjector, value);
    }

    public string SelectedBuilding
    {
      get => _selectedBuilding;
      set => SetProperty(ref _selectedBuilding, value);
    }

    private string _selectedType = "Всі";
    private int _minCapacity;

    public string SelectedType
    {
      get => _selectedType;
      set
      {
        SetProperty(ref _selectedType, value);
        // Автоматичний пошук при зміні фільтрів (опціонально)
        // ExecuteSearch(null);
      }
    }

    public int MinCapacity
    {
      get => _minCapacity;
      set
      {
        SetProperty(ref _minCapacity, value);
        // Автоматичний пошук при зміні фільтрів (опціонально)
        // ExecuteSearch(null);
      }
    }

    public bool IsSearching
    {
      get => _isSearching;
      set
      {
        SetProperty(ref _isSearching, value);
        SearchCommand.RaiseCanExecuteChanged();
      }
    }

    public ObservableCollection<string> Buildings { get; }
    public ObservableCollection<string> ClassroomTypes { get; }
    public ObservableCollection<ClassroomItem> SearchResults { get; }
    public ObservableCollection<ClassroomItem> AllClassrooms { get; }

    public RelayCommand SearchCommand { get; }
    public RelayCommand UseMyScheduleCommand { get; }

    private bool CanExecuteSearch(object parameter)
    {
      return !IsSearching &&
             StartTime < EndTime &&
             StartTime >= DateTime.Today;
    }

    private async void ExecuteSearch(object parameter)
    {
      try
      {
        IsSearching = true;
        SearchResults.Clear();

        // Імітація затримки пошуку
        await System.Threading.Tasks.Task.Delay(500);

        var availableClassrooms = AllClassrooms.Where(c =>
            (SelectedBuilding == "Всі" || c.Building == SelectedBuilding) &&
            (SelectedType == "Всі" || c.Type == SelectedType) &&
            (!HasComputers || c.HasComputers) &&
            (!HasProjector || c.HasProjector) &&
            (MinCapacity == 0 || c.Capacity >= MinCapacity)
        ).ToList();

        // Імітація перевірки
        var random = new Random();
        foreach (var classroom in availableClassrooms)
        {
          // рандомимо вільна аудиторія чи ні для демонстрації
          bool isFree = random.Next(0, 2) == 0;

          if (isFree)
          {
            SearchResults.Add(classroom);
          }
        }

        if (SearchResults.Any())
        {
          Debug.WriteLine($"Знайдено {SearchResults.Count} вільних аудиторій");
        }
        else
        {
          Debug.WriteLine("Вільних аудиторій не знайдено");
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Помилка пошуку: {ex.Message}");
      }
      finally
      {
        IsSearching = false;
      }
    }

    private void ExecuteUseMySchedule(object parameter)
    {

      var now = DateTime.Now;
      StartTime = now.Date.AddHours(14).AddMinutes(0); // 14:00
      EndTime = now.Date.AddHours(15).AddMinutes(30);  // 15:30

      Debug.WriteLine("Вибрано час з розкладу: 14:00 - 15:30");
    }

    // швидкий пошук
    public void SearchForNow()
    {
      StartTime = DateTime.Now;
      EndTime = DateTime.Now.AddHours(2);
      ExecuteSearch(null);
    }

    // пошук на день/час
    public void SearchForDate(DateTime date)
    {
      StartTime = date.Date.AddHours(8); // 08:00
      EndTime = date.Date.AddHours(20);  // 20:00
      ExecuteSearch(null);
    }
  }
}