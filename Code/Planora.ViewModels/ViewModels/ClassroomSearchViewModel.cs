using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Важливо

namespace Planora.ViewModels.ViewModels
{
    public class ClassroomDto
    {
        public string Number { get; set; } = "";
        public string Building { get; set; } = "";
        public string Type { get; set; } = ""; 
        public int Capacity { get; set; }
        public bool HasComputers { get; set; }
        public bool HasProjector { get; set; }
        public bool IsFree { get; set; } = true;
    }

    public class ClassroomSearchViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private ClassroomDto? _selectedClassroom;
        private bool _isSearching;
        
        // Фільтри
        private string _selectedBuilding = "Всі";
        private string _selectedType = "Всі";
        private bool _hasComputers;
        private bool _hasProjector;

        // "База даних"
        private List<ClassroomDto> _allClassrooms = new();

        public ClassroomSearchViewModel()
        {
            // 👇 Ініціалізація колекцій (обов'язково!)
            Buildings = new ObservableCollection<string> { "Всі", "Корпус А", "Корпус Б", "Корпус В" };
            ClassroomTypes = new ObservableCollection<string> { "Всі", "Лекційна", "Практична", "Лабораторна" };
            SearchResults = new ObservableCollection<ClassroomDto>();
            
            SearchCommand = new RelayCommand(ExecuteSearch);
            OpenDetailCommand = new RelayCommand(ExecuteOpenDetail);
            CloseDetailCommand = new RelayCommand(_ => SelectedClassroom = null);

            // Генеруємо дані
            InitializeData();
            
            // Перший пошук
            FilterData();
        }

        // --- Властивості ---
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); FilterData(); }
        }

        public string SelectedBuilding
        {
            get => _selectedBuilding;
            set { SetProperty(ref _selectedBuilding, value); FilterData(); }
        }

        public string SelectedType
        {
            get => _selectedType;
            set { SetProperty(ref _selectedType, value); FilterData(); }
        }

        public bool HasComputers
        {
            get => _hasComputers;
            set { SetProperty(ref _hasComputers, value); FilterData(); }
        }

        public bool HasProjector
        {
            get => _hasProjector;
            set { SetProperty(ref _hasProjector, value); FilterData(); }
        }

        public ClassroomDto? SelectedClassroom
        {
            get => _selectedClassroom;
            set => SetProperty(ref _selectedClassroom, value);
        }

        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }

        // --- Колекції ---
        public ObservableCollection<string> Buildings { get; }
        public ObservableCollection<string> ClassroomTypes { get; }
        public ObservableCollection<ClassroomDto> SearchResults { get; }
        
        public RelayCommand SearchCommand { get; }
        public RelayCommand OpenDetailCommand { get; }
        public RelayCommand CloseDetailCommand { get; }

        // --- Методи ---

        private void InitializeData()
        {
            var rand = new Random();
            for (int i = 101; i < 130; i++)
            {
                _allClassrooms.Add(new ClassroomDto { 
                    Number = i.ToString(), 
                    Building = i < 115 ? "Корпус А" : "Корпус Б", 
                    Type = i % 2 == 0 ? "Лекційна" : "Лабораторна", 
                    Capacity = rand.Next(20, 100),
                    HasComputers = i % 3 == 0,
                    HasProjector = i % 4 == 0,
                    IsFree = i % 5 != 0
                });
            }
        }

        // Цей метод використовується для кнопки "Знайти" (з імітацією затримки)
        private async void ExecuteSearch(object parameter)
        {
            IsSearching = true;
            await Task.Delay(300);
            FilterData();
            IsSearching = false;
        }

        // Цей метод миттєво фільтрує локальний список
        private void FilterData()
        {
            SearchResults.Clear();
            
            var filtered = _allClassrooms.Where(c => 
                (string.IsNullOrEmpty(SearchText) || c.Number.Contains(SearchText)) &&
                (SelectedBuilding == "Всі" || c.Building == SelectedBuilding) &&
                (SelectedType == "Всі" || c.Type == SelectedType) &&
                (!HasComputers || c.HasComputers) &&
                (!HasProjector || c.HasProjector)
            );

            foreach (var item in filtered) SearchResults.Add(item);
        }

        private void ExecuteOpenDetail(object parameter)
        {
            if (parameter is ClassroomDto classroom)
            {
                SelectedClassroom = classroom;
            }
        }
    }
}