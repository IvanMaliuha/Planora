using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace Planora.ViewModels.ViewModels
{
    public class ClassroomManagementViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private string _selectedFilter = "За номером";
        private ClassroomItem _selectedClassroom;
        private bool _isEditMode;

        public ClassroomManagementViewModel()
        {
            Classrooms = new ObservableCollection<ClassroomItem>
            {
                new ClassroomItem { Number = "101", Building = "A", Type = "Лекційна", Capacity = 50, HasComputers = true, HasProjector = true },
                new ClassroomItem { Number = "201", Building = "B", Type = "Лабораторна", Capacity = 25, HasComputers = true, HasProjector = false },
                new ClassroomItem { Number = "301", Building = "A", Type = "Практична", Capacity = 30, HasComputers = false, HasProjector = true }
            };

            FilterOptions = new ObservableCollection<string>
            {
                "За номером",
                "За корпусом",
                "За типом"
            };

            // Команди
            AddClassroomCommand = new RelayCommand(ExecuteAddClassroom);
            EditClassroomCommand = new RelayCommand(ExecuteEditClassroom, CanExecuteEditDelete);
            DeleteClassroomCommand = new RelayCommand(ExecuteDeleteClassroom, CanExecuteEditDelete);
            SaveClassroomCommand = new RelayCommand(ExecuteSaveClassroom, CanExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            // Нова аудиторія для форми
            NewClassroom = new ClassroomItem();
        }

        public ObservableCollection<ClassroomItem> Classrooms { get; }
        public ObservableCollection<string> FilterOptions { get; }

        public string SearchText
        {
            get => _searchText;
            set 
            { 
                SetProperty(ref _searchText, value);
                FilterClassrooms();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set 
            { 
                SetProperty(ref _selectedFilter, value);
                FilterClassrooms();
            }
        }

        public ClassroomItem SelectedClassroom
        {
            get => _selectedClassroom;
            set 
            { 
                SetProperty(ref _selectedClassroom, value);
                EditClassroomCommand.RaiseCanExecuteChanged();
                DeleteClassroomCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public ClassroomItem NewClassroom { get; }

        // Команди
        public RelayCommand AddClassroomCommand { get; }
        public RelayCommand EditClassroomCommand { get; }
        public RelayCommand DeleteClassroomCommand { get; }
        public RelayCommand SaveClassroomCommand { get; }
        public RelayCommand CancelCommand { get; }

        // Фільтрація аудиторій
        private void FilterClassrooms()
        {
            // Тут буде логіка фільтрації, коли буде реалізовано UI
            System.Diagnostics.Debug.WriteLine($"Пошук: {SearchText}, Фільтр: {SelectedFilter}");
        }

        // Перевірка можливості виконання команд редагування/видалення
        private bool CanExecuteEditDelete(object parameter)
        {
            return SelectedClassroom != null && !IsEditMode;
        }

        // Перевірка можливості збереження
        private bool CanExecuteSave(object parameter)
        {
            return IsEditMode && 
                   !string.IsNullOrWhiteSpace(NewClassroom.Number) &&
                   !string.IsNullOrWhiteSpace(NewClassroom.Building) &&
                   !string.IsNullOrWhiteSpace(NewClassroom.Type) &&
                   NewClassroom.Capacity > 0;
        }

        // Додавання нової аудиторії
        private void ExecuteAddClassroom(object parameter)
        {
            IsEditMode = true;
            NewClassroom.Number = string.Empty;
            NewClassroom.Building = string.Empty;
            NewClassroom.Type = string.Empty;
            NewClassroom.Capacity = 0;
            NewClassroom.HasComputers = false;
            NewClassroom.HasProjector = false;
            
            SaveClassroomCommand.RaiseCanExecuteChanged();
            System.Diagnostics.Debug.WriteLine("Режим додавання нової аудиторії");
        }

        // Редагування існуючої аудиторії
        private void ExecuteEditClassroom(object parameter)
        {
            if (SelectedClassroom != null)
            {
                IsEditMode = true;
                
                // Копіюємо дані вибраної аудиторії в форму редагування
                NewClassroom.Number = SelectedClassroom.Number;
                NewClassroom.Building = SelectedClassroom.Building;
                NewClassroom.Type = SelectedClassroom.Type;
                NewClassroom.Capacity = SelectedClassroom.Capacity;
                NewClassroom.HasComputers = SelectedClassroom.HasComputers;
                NewClassroom.HasProjector = SelectedClassroom.HasProjector;
                
                SaveClassroomCommand.RaiseCanExecuteChanged();
                System.Diagnostics.Debug.WriteLine($"Режим редагування аудиторії: {SelectedClassroom.Number}");
            }
        }

        // Видалення аудиторії
        private void ExecuteDeleteClassroom(object parameter)
        {
            if (SelectedClassroom != null)
            {
                Classrooms.Remove(SelectedClassroom);
                System.Diagnostics.Debug.WriteLine($"Аудиторія видалена: {SelectedClassroom.Number}");
                SelectedClassroom = null;
            }
        }

        // Збереження змін (додавання або редагування)
        private void ExecuteSaveClassroom(object parameter)
        {
            try
            {
                if (IsEditMode)
                {
                    // Перевіряємо, чи це новий запис чи редагування існуючого
                    var existingClassroom = Classrooms.FirstOrDefault(c => c.Number == NewClassroom.Number && c.Building == NewClassroom.Building);
                    
                    if (existingClassroom != null && existingClassroom != SelectedClassroom)
                    {
                        // Аудиторія з таким номером і корпусом вже існує
                        System.Diagnostics.Debug.WriteLine("Помилка: Аудиторія з таким номером і корпусом вже існує");
                        return;
                    }

                    if (SelectedClassroom == null)
                    {
                        // Додавання нової аудиторії
                        Classrooms.Add(new ClassroomItem
                        {
                            Number = NewClassroom.Number,
                            Building = NewClassroom.Building,
                            Type = NewClassroom.Type,
                            Capacity = NewClassroom.Capacity,
                            HasComputers = NewClassroom.HasComputers,
                            HasProjector = NewClassroom.HasProjector
                        });
                        System.Diagnostics.Debug.WriteLine($"Аудиторія додана: {NewClassroom.Number}");
                    }
                    else
                    {
                        // Редагування існуючої аудиторії
                        SelectedClassroom.Number = NewClassroom.Number;
                        SelectedClassroom.Building = NewClassroom.Building;
                        SelectedClassroom.Type = NewClassroom.Type;
                        SelectedClassroom.Capacity = NewClassroom.Capacity;
                        SelectedClassroom.HasComputers = NewClassroom.HasComputers;
                        SelectedClassroom.HasProjector = NewClassroom.HasProjector;
                        System.Diagnostics.Debug.WriteLine($"Аудиторія оновлена: {NewClassroom.Number}");
                    }

                    IsEditMode = false;
                    SelectedClassroom = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка при збереженні аудиторії: {ex.Message}");
            }
        }

        // Скасування операції
        private void ExecuteCancel(object parameter)
        {
            IsEditMode = false;
            SelectedClassroom = null;
            System.Diagnostics.Debug.WriteLine("Операцію скасовано");
        }
    }

    public class ClassroomItem : ViewModelBase
    {
        private string _number = string.Empty;
        private string _building = string.Empty;
        private string _type = string.Empty;
        private int _capacity;
        private bool _hasComputers;
        private bool _hasProjector;

        public string Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public string Building
        {
            get => _building;
            set => SetProperty(ref _building, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public int Capacity
        {
            get => _capacity;
            set => SetProperty(ref _capacity, value);
        }

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
    }
}