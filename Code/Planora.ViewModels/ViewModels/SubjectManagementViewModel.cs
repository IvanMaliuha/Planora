using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace Planora.ViewModels.ViewModels
{
    public class SubjectManagementViewModel : ViewModelBase
    {
        private SubjectItem _selectedSubject;
        private bool _isEditMode;
        private string _searchText = string.Empty;

        public SubjectManagementViewModel()
        {
            Subjects = new ObservableCollection<SubjectItem>
            {
                new SubjectItem { Name = "Математика", Type = "Лекція", Credits = 5 },
                new SubjectItem { Name = "Програмування", Type = "Практика", Credits = 6 },
                new SubjectItem { Name = "Бази даних", Type = "Лабораторна", Credits = 4 }
            };

            AddSubjectCommand = new RelayCommand(ExecuteAddSubject);
            EditSubjectCommand = new RelayCommand(ExecuteEditSubject, CanExecuteEditDelete);
            DeleteSubjectCommand = new RelayCommand(ExecuteDeleteSubject, CanExecuteEditDelete);
            SaveSubjectCommand = new RelayCommand(ExecuteSaveSubject);
            CancelCommand = new RelayCommand(ExecuteCancel);

            NewSubject = new SubjectItem();
        }

        public ObservableCollection<SubjectItem> Subjects { get; }
        
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); }
        }

        public SubjectItem SelectedSubject
        {
            get => _selectedSubject;
            set 
            { 
                SetProperty(ref _selectedSubject, value);
                EditSubjectCommand.RaiseCanExecuteChanged();
                DeleteSubjectCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public SubjectItem NewSubject { get; }

        public RelayCommand AddSubjectCommand { get; }
        public RelayCommand EditSubjectCommand { get; }
        public RelayCommand DeleteSubjectCommand { get; }
        public RelayCommand SaveSubjectCommand { get; }
        public RelayCommand CancelCommand { get; }

        private bool CanExecuteEditDelete(object parameter) => SelectedSubject != null && !IsEditMode;

        private void ExecuteAddSubject(object parameter)
        {
            IsEditMode = true;
            SelectedSubject = null;
            NewSubject.Name = string.Empty;
            NewSubject.Type = string.Empty;
            NewSubject.Credits = 0;
        }

        private void ExecuteEditSubject(object parameter)
        {
            if (SelectedSubject != null)
            {
                IsEditMode = true;
                NewSubject.Name = SelectedSubject.Name;
                NewSubject.Type = SelectedSubject.Type;
                NewSubject.Credits = SelectedSubject.Credits;
            }
        }

        private void ExecuteDeleteSubject(object parameter)
        {
            if (SelectedSubject != null)
            {
                Subjects.Remove(SelectedSubject);
                SelectedSubject = null;
            }
        }

        private void ExecuteSaveSubject(object parameter)
        {
            if (SelectedSubject != null)
            {
                SelectedSubject.Name = NewSubject.Name;
                SelectedSubject.Type = NewSubject.Type;
                SelectedSubject.Credits = NewSubject.Credits;
            }
            else
            {
                Subjects.Add(new SubjectItem 
                { 
                    Name = NewSubject.Name, 
                    Type = NewSubject.Type,
                    Credits = NewSubject.Credits
                });
            }
            IsEditMode = false;
            SelectedSubject = null;
        }

        private void ExecuteCancel(object parameter)
        {
            IsEditMode = false;
            SelectedSubject = null;
        }
    }

    public class SubjectItem : ViewModelBase
    {
        private string _name = string.Empty;
        private string _type = string.Empty;
        private int _credits;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Type { get => _type; set => SetProperty(ref _type, value); }
        public int Credits { get => _credits; set => SetProperty(ref _credits, value); }
    }
}