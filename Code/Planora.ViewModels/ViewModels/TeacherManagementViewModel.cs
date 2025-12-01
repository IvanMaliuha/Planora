using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace Planora.ViewModels.ViewModels
{
    public class TeacherManagementViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private TeacherItem _selectedTeacher;
        private bool _isEditMode;

        public TeacherManagementViewModel()
        {
            // Тестові дані
            Teachers = new ObservableCollection<TeacherItem>
            {
                new TeacherItem { FullName = "Іванов Іван Іванович", Department = "Комп'ютерні науки", Position = "Професор", Email = "ivanov@univ.ua" },
                new TeacherItem { FullName = "Петрова Марія Василівна", Department = "Математика", Position = "Доцент", Email = "petrova@univ.ua" },
                new TeacherItem { FullName = "Сидоров Петро Петрович", Department = "ПЗ", Position = "Асистент", Email = "sydorov@univ.ua" }
            };

            // Команди
            AddTeacherCommand = new RelayCommand(ExecuteAddTeacher);
            EditTeacherCommand = new RelayCommand(ExecuteEditTeacher, CanExecuteEditDelete);
            DeleteTeacherCommand = new RelayCommand(ExecuteDeleteTeacher, CanExecuteEditDelete);
            SaveTeacherCommand = new RelayCommand(ExecuteSaveTeacher); // Завжди активна, щоб уникнути багів
            CancelCommand = new RelayCommand(ExecuteCancel);

            NewTeacher = new TeacherItem();
        }

        public ObservableCollection<TeacherItem> Teachers { get; }

        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); /* Тут можна додати логіку фільтрації */ }
        }

        public TeacherItem SelectedTeacher
        {
            get => _selectedTeacher;
            set 
            { 
                SetProperty(ref _selectedTeacher, value);
                EditTeacherCommand.RaiseCanExecuteChanged();
                DeleteTeacherCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        // Об'єкт для редагування/створення
        public TeacherItem NewTeacher { get; }

        public RelayCommand AddTeacherCommand { get; }
        public RelayCommand EditTeacherCommand { get; }
        public RelayCommand DeleteTeacherCommand { get; }
        public RelayCommand SaveTeacherCommand { get; }
        public RelayCommand CancelCommand { get; }

        private bool CanExecuteEditDelete(object parameter) => SelectedTeacher != null && !IsEditMode;

        private void ExecuteAddTeacher(object parameter)
        {
            IsEditMode = true;
            SelectedTeacher = null; // Знімаємо виділення
            
            // Очищаємо форму
            NewTeacher.FullName = string.Empty;
            NewTeacher.Department = string.Empty;
            NewTeacher.Position = string.Empty;
            NewTeacher.Email = string.Empty;
        }

        private void ExecuteEditTeacher(object parameter)
        {
            if (SelectedTeacher != null)
            {
                IsEditMode = true;
                // Копіюємо дані у форму
                NewTeacher.FullName = SelectedTeacher.FullName;
                NewTeacher.Department = SelectedTeacher.Department;
                NewTeacher.Position = SelectedTeacher.Position;
                NewTeacher.Email = SelectedTeacher.Email;
            }
        }

        private void ExecuteDeleteTeacher(object parameter)
        {
            if (SelectedTeacher != null)
            {
                Teachers.Remove(SelectedTeacher);
                SelectedTeacher = null;
            }
        }

        private void ExecuteSaveTeacher(object parameter)
        {
            // Якщо редагуємо існуючого
            if (SelectedTeacher != null)
            {
                SelectedTeacher.FullName = NewTeacher.FullName;
                SelectedTeacher.Department = NewTeacher.Department;
                SelectedTeacher.Position = NewTeacher.Position;
                SelectedTeacher.Email = NewTeacher.Email;
            }
            // Якщо створюємо нового
            else
            {
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
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditMode = false;
            SelectedTeacher = null;
        }
    }

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