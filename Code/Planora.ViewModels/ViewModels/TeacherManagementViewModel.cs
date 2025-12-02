using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Collections.Generic; 
using System.Linq;
using System;
using Serilog; 

namespace Planora.ViewModels.ViewModels
{
    public class TeacherManagementViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private TeacherItem? _selectedTeacher;
        private bool _isEditMode;

        public List<string> AvailablePositions { get; } = new List<string>
        {
            "Асистент",
            "Викладач",
            "Старший викладач",
            "Доцент",
            "Завідувач кафедри"
        };

        // Колекція викладачів
        public ObservableCollection<TeacherItem> Teachers { get; set; }

        // Об'єкт для створення/редагування
        public TeacherItem NewTeacher { get; set; } = new TeacherItem();

        public TeacherManagementViewModel()
        {
            Log.Debug("Відкрито сторінку управління викладачами.");


            Teachers = new ObservableCollection<TeacherItem>
            {
                new TeacherItem 
                { 
                    FullName = "Іванов Іван Іванович", 
                    Department = "Комп'ютерні науки", 
                    Position = "Професор", 
                    Email = "ivanov@univ.ua" 
                },
                new TeacherItem 
                { 
                    FullName = "Петрова Марія Василівна", 
                    Department = "Математика", 
                    Position = "Доцент", 
                    Email = "petrova@univ.ua" 
                },
                new TeacherItem 
                { 
                    FullName = "Сидоров Петро Петрович", 
                    Department = "ПЗ", 
                    Position = "Асистент", 
                    Email = "sydorov@univ.ua" 
                }
            };
            Log.Debug("Завантажено {Count} викладачів із тестового джерела.", Teachers.Count);

            // Ініціалізація команд
            AddTeacherCommand = new RelayCommand(ExecuteAddTeacher);
            EditTeacherCommand = new RelayCommand(ExecuteEditTeacher, CanExecuteEditDelete);
            DeleteTeacherCommand = new RelayCommand(ExecuteDeleteTeacher, CanExecuteEditDelete);
            SaveTeacherCommand = new RelayCommand(ExecuteSaveTeacher);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        // Властивості
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                Log.Debug("Фільтрація списку за запитом: '{Query}'", value); 
            }
        }

        public TeacherItem? SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                SetProperty(ref _selectedTeacher, value);
                EditTeacherCommand.RaiseCanExecuteChanged();
                DeleteTeacherCommand.RaiseCanExecuteChanged();
                
                if (value != null)
                    Log.Debug("Вибрано викладача: {Name}", value.FullName);
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // Команди
        public RelayCommand AddTeacherCommand { get; }
        public RelayCommand EditTeacherCommand { get; }
        public RelayCommand DeleteTeacherCommand { get; }
        public RelayCommand SaveTeacherCommand { get; }
        public RelayCommand CancelCommand { get; }

        private bool CanExecuteEditDelete(object parameter) => SelectedTeacher != null;

        // --- Реалізація команд ---

        private void ExecuteAddTeacher(object parameter)
        {
            Log.Information("Користувач натиснув 'Додати викладача'. Відкриття форми.");
            NewTeacher = new TeacherItem();
            OnPropertyChanged(nameof(NewTeacher)); 
            IsEditMode = true;
            SelectedTeacher = null;
        }

        private void ExecuteEditTeacher(object parameter)
        {
            if (SelectedTeacher == null) return;

            Log.Information("Користувач почав редагування викладача: {Name}", SelectedTeacher.FullName);
            
            NewTeacher = new TeacherItem
            {
                FullName = SelectedTeacher.FullName,
                Department = SelectedTeacher.Department,
                Position = SelectedTeacher.Position,
                Email = SelectedTeacher.Email
            };
            OnPropertyChanged(nameof(NewTeacher));
            IsEditMode = true;
        }

        private void ExecuteDeleteTeacher(object parameter)
        {
            if (SelectedTeacher != null)
            {
                Log.Information("ВИДАЛЕННЯ: Користувач видалив викладача {Name} ({Email}).", SelectedTeacher.FullName, SelectedTeacher.Email);
                Teachers.Remove(SelectedTeacher);
                SelectedTeacher = null;
            }
        }

        private void ExecuteSaveTeacher(object parameter)
        {
            // Валідація: Ім'я не порожнє
            if (string.IsNullOrWhiteSpace(NewTeacher.FullName))
            {
                Log.Warning("Невдала спроба збереження: Поле ПІБ порожнє.");
                return;
            }

            // Валідація: Посада обрана
            if (string.IsNullOrWhiteSpace(NewTeacher.Position))
            {
                Log.Warning("Невдала спроба збереження: Посада не обрана.");
                return;
            }

            try
            {
                if (SelectedTeacher != null)
                {
                    // Режим редагування
                    Log.Information("Збереження змін для викладача: {OldName} -> {NewName}", SelectedTeacher.FullName, NewTeacher.FullName);
                    
                    SelectedTeacher.FullName = NewTeacher.FullName;
                    SelectedTeacher.Department = NewTeacher.Department;
                    SelectedTeacher.Position = NewTeacher.Position;
                    SelectedTeacher.Email = NewTeacher.Email;
                }
                else
                {
                    // Режим додавання
                    Log.Information("СТВОРЕНО НОВОГО ВИКЛАДАЧА: {Name}, Посада: {Pos}", NewTeacher.FullName, NewTeacher.Position);
                    
                    Teachers.Add(new TeacherItem
                    {
                        FullName = NewTeacher.FullName,
                        Department = NewTeacher.Department,
                        Position = NewTeacher.Position,
                        Email = NewTeacher.Email
                    });
                }

                IsEditMode = false;
                SelectedTeacher = null;
                Log.Information("Дані успішно збережено.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Критична помилка при збереженні викладача {Name}", NewTeacher.FullName);
            }
        }

        private void ExecuteCancel(object parameter)
        {
            Log.Information("Користувач скасував редагування/додавання.");
            IsEditMode = false;
            SelectedTeacher = null;
        }
    }

    // Модель даних
    public class TeacherItem : ViewModelBase
    {
        private string _fullName = string.Empty;
        private string _department = string.Empty;
        private string _position = string.Empty;
        private string _email = string.Empty;

        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Department { get => _department; set => SetProperty(ref _department, value); }
        public string Position { get => _position; set => SetProperty(ref _position, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
    }
}