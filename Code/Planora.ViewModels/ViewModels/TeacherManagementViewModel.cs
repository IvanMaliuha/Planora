using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;

namespace Planora.ViewModels.ViewModels
{
  public class TeacherManagementViewModel : ViewModelBase
  {
    public TeacherManagementViewModel()
    {
      Teachers = new ObservableCollection<TeacherItem>
            {
                new TeacherItem { FullName = "Іванов Іван Іванович", Department = "Комп'ютерні науки", Position = "Професор" },
                new TeacherItem { FullName = "Петрова Марія Василівна", Department = "Математика", Position = "Доцент" }
            };

      AddTeacherCommand = new RelayCommand(ExecuteAddTeacher);
      EditTeacherCommand = new RelayCommand(ExecuteEditTeacher);
      DeleteTeacherCommand = new RelayCommand(ExecuteDeleteTeacher);
    }

    public ObservableCollection<TeacherItem> Teachers { get; }
    public RelayCommand AddTeacherCommand { get; }
    public RelayCommand EditTeacherCommand { get; }
    public RelayCommand DeleteTeacherCommand { get; }

    private void ExecuteAddTeacher(object parameter) => System.Diagnostics.Debug.WriteLine("Додати викладача");
    private void ExecuteEditTeacher(object parameter) => System.Diagnostics.Debug.WriteLine("Редагувати викладача");
    private void ExecuteDeleteTeacher(object parameter) => System.Diagnostics.Debug.WriteLine("Видалити викладача");
  }

  public class TeacherItem : ViewModelBase
  {
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
  }
}