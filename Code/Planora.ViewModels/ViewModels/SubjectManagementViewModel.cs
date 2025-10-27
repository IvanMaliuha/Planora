using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace Planora.ViewModels.ViewModels
{
    public class SubjectManagementViewModel : ViewModelBase
    {
        public SubjectManagementViewModel()
        {
            Subjects = new ObservableCollection<SubjectItem>
            {
                new SubjectItem { Name = "Математика", Type = "Лекція" },
                new SubjectItem { Name = "Програмування", Type = "Практика" },
                new SubjectItem { Name = "Бази даних", Type = "Лабораторна" }
            };

            AddSubjectCommand = new RelayCommand(ExecuteAddSubject);
            EditSubjectCommand = new RelayCommand(ExecuteEditSubject);
            DeleteSubjectCommand = new RelayCommand(ExecuteDeleteSubject);
        }

        public ObservableCollection<SubjectItem> Subjects { get; }
        public RelayCommand AddSubjectCommand { get; }
        public RelayCommand EditSubjectCommand { get; }
        public RelayCommand DeleteSubjectCommand { get; }

        private void ExecuteAddSubject(object parameter) => System.Diagnostics.Debug.WriteLine("Додати предмет");
        private void ExecuteEditSubject(object parameter) => System.Diagnostics.Debug.WriteLine("Редагувати предмет");
        private void ExecuteDeleteSubject(object parameter) => System.Diagnostics.Debug.WriteLine("Видалити предмет");
    }

    public class SubjectItem : ViewModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}