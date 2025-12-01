using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace Planora.ViewModels.ViewModels
{
    public class GroupManagementViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        private GroupItem _selectedGroup;
        private bool _isEditMode;

        public GroupManagementViewModel()
        {
            // Тестові дані
            Groups = new ObservableCollection<GroupItem>
            {
                new GroupItem { Name = "КН-101", Faculty = "Комп'ютерні науки", Course = 1, StudentCount = 30 },
                new GroupItem { Name = "КН-102", Faculty = "Комп'ютерні науки", Course = 1, StudentCount = 28 },
                new GroupItem { Name = "ПЗ-201", Faculty = "Програмна інженерія", Course = 2, StudentCount = 25 }
            };

            AddGroupCommand = new RelayCommand(ExecuteAddGroup);
            EditGroupCommand = new RelayCommand(ExecuteEditGroup, CanExecuteEditDelete);
            DeleteGroupCommand = new RelayCommand(ExecuteDeleteGroup, CanExecuteEditDelete);
            SaveGroupCommand = new RelayCommand(ExecuteSaveGroup);
            CancelCommand = new RelayCommand(ExecuteCancel);

            NewGroup = new GroupItem();
        }

        public ObservableCollection<GroupItem> Groups { get; }

        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); /* Тут можна додати фільтр */ }
        }

        public GroupItem SelectedGroup
        {
            get => _selectedGroup;
            set 
            { 
                SetProperty(ref _selectedGroup, value);
                EditGroupCommand.RaiseCanExecuteChanged();
                DeleteGroupCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public GroupItem NewGroup { get; }

        public RelayCommand AddGroupCommand { get; }
        public RelayCommand EditGroupCommand { get; }
        public RelayCommand DeleteGroupCommand { get; }
        public RelayCommand SaveGroupCommand { get; }
        public RelayCommand CancelCommand { get; }

        private bool CanExecuteEditDelete(object parameter) => SelectedGroup != null && !IsEditMode;

        private void ExecuteAddGroup(object parameter)
        {
            IsEditMode = true;
            SelectedGroup = null;
            
            NewGroup.Name = string.Empty;
            NewGroup.Faculty = string.Empty;
            NewGroup.Course = 1;
            NewGroup.StudentCount = 0;
        }

        private void ExecuteEditGroup(object parameter)
        {
            if (SelectedGroup != null)
            {
                IsEditMode = true;
                NewGroup.Name = SelectedGroup.Name;
                NewGroup.Faculty = SelectedGroup.Faculty;
                NewGroup.Course = SelectedGroup.Course;
                NewGroup.StudentCount = SelectedGroup.StudentCount;
            }
        }

        private void ExecuteDeleteGroup(object parameter)
        {
            if (SelectedGroup != null)
            {
                Groups.Remove(SelectedGroup);
                SelectedGroup = null;
            }
        }

        private void ExecuteSaveGroup(object parameter)
        {
            if (SelectedGroup != null)
            {
                SelectedGroup.Name = NewGroup.Name;
                SelectedGroup.Faculty = NewGroup.Faculty;
                SelectedGroup.Course = NewGroup.Course;
                SelectedGroup.StudentCount = NewGroup.StudentCount;
            }
            else
            {
                Groups.Add(new GroupItem
                {
                    Name = NewGroup.Name,
                    Faculty = NewGroup.Faculty,
                    Course = NewGroup.Course,
                    StudentCount = NewGroup.StudentCount
                });
            }
            IsEditMode = false;
            SelectedGroup = null;
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditMode = false;
            SelectedGroup = null;
        }
    }

    public class GroupItem : ViewModelBase
    {
        private string _name = string.Empty;
        private string _faculty = string.Empty;
        private int _course;
        private int _studentCount;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Faculty { get => _faculty; set => SetProperty(ref _faculty, value); }
        public int Course { get => _course; set => SetProperty(ref _course, value); }
        public int StudentCount { get => _studentCount; set => SetProperty(ref _studentCount, value); }
    }
}