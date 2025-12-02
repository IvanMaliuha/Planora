using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Collections.Generic; // Для List
using Serilog;

namespace Planora.ViewModels.ViewModels
{
    public class SubjectManagementViewModel : ViewModelBase
    {
        private SubjectItem? _selectedSubject;
        private bool _isEditMode;
        private string _searchText = "";

        public List<string> AvailableSubjectTypes { get; } = new List<string>
        {
            "Лекція",
            "Практика",
            "Лабораторна",
            "Семінар"
        };

        public ObservableCollection<SubjectItem> Subjects { get; set; }
        public SubjectItem NewSubject { get; set; } = new SubjectItem();

        public RelayCommand AddSubjectCommand { get; }
        public RelayCommand EditSubjectCommand { get; }
        public RelayCommand DeleteSubjectCommand { get; }
        public RelayCommand SaveSubjectCommand { get; }
        public RelayCommand CancelCommand { get; }

        public SubjectManagementViewModel()
        {
            Subjects = new ObservableCollection<SubjectItem>
            {
                new SubjectItem { Name = "Вища математика", Type = "Лекція", Credits = 5 },
                new SubjectItem { Name = "Програмування", Type = "Лабораторна", Credits = 6 }
            };

            AddSubjectCommand = new RelayCommand(_ => { 
                NewSubject = new SubjectItem(); 
                OnPropertyChanged(nameof(NewSubject));
                IsEditMode = true; 
                SelectedSubject = null; 
            });

            EditSubjectCommand = new RelayCommand(_ => {
                if (SelectedSubject == null) return;
                NewSubject = new SubjectItem 
                { 
                    Name = SelectedSubject.Name, 
                    Type = SelectedSubject.Type, 
                    Credits = SelectedSubject.Credits 
                };
                OnPropertyChanged(nameof(NewSubject));
                IsEditMode = true;
            }, _ => SelectedSubject != null);

            DeleteSubjectCommand = new RelayCommand(_ => {
                if (SelectedSubject != null) Subjects.Remove(SelectedSubject);
            }, _ => SelectedSubject != null);

            SaveSubjectCommand = new RelayCommand(_ => {
                if (string.IsNullOrEmpty(NewSubject.Type)) {
                    Log.Warning("Тип предмету не обрано!");
                    return;
                }

                if (SelectedSubject != null) // Редагування
                {
                    SelectedSubject.Name = NewSubject.Name;
                    SelectedSubject.Type = NewSubject.Type;
                    SelectedSubject.Credits = NewSubject.Credits;
                }
                else // Додавання
                {
                    Subjects.Add(new SubjectItem 
                    { 
                        Name = NewSubject.Name, 
                        Type = NewSubject.Type, 
                        Credits = NewSubject.Credits 
                    });
                }
                IsEditMode = false;
                Log.Information("Предмет збережено: {Name} ({Type})", NewSubject.Name, NewSubject.Type);
            });

            CancelCommand = new RelayCommand(_ => IsEditMode = false);
        }

        public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }
        
        public bool IsEditMode { get => _isEditMode; set => SetProperty(ref _isEditMode, value); }

        public SubjectItem? SelectedSubject
        {
            get => _selectedSubject;
            set 
            { 
                SetProperty(ref _selectedSubject, value);
                EditSubjectCommand.RaiseCanExecuteChanged();
                DeleteSubjectCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public class SubjectItem : ViewModelBase
    {
        private string _name = "";
        private string _type = "";
        private int _credits;

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Type { get => _type; set => SetProperty(ref _type, value); }
        public int Credits { get => _credits; set => SetProperty(ref _credits, value); }
    }
}