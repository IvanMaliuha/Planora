using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Planora.ViewModels.ViewModels
{
    public class ScheduleGenerationViewModel : ViewModelBase
    {
        private string _selectedTeacher;
        private string _selectedBuilding = "Всі";
        private bool _isGenerating;
        private bool _hasGeneratedSchedule;

        public ScheduleGenerationViewModel()
        {
            // Ініціалізація колекцій
            Teachers = new ObservableCollection<string> 
            { 
                "Іванов І.І.", 
                "Петрова М.В.", 
                "Сидоров О.П.", 
                "Коваленко Т.С." 
            };
            
            Subjects = new ObservableCollection<string> 
            { 
                "Математика", 
                "Програмування", 
                "Бази даних", 
                "Фізика" 
            };
            
            Groups = new ObservableCollection<string> 
            { 
                "КН-101", 
                "КН-102", 
                "КН-201", 
                "КН-202" 
            };
            
            Buildings = new ObservableCollection<string> 
            { 
                "Всі", 
                "Корпус A", 
                "Корпус B", 
                "Корпус C" 
            };
            
            TeacherWorkload = new ObservableCollection<WorkloadItem>();
            GeneratedSchedule = new ObservableCollection<ScheduleItem>();

            // Команди
            AddTeacherCommand = new RelayCommand(ExecuteAddTeacher, CanExecuteAddTeacher);
            AddWorkloadRowCommand = new RelayCommand(ExecuteAddWorkloadRow, CanExecuteAddWorkloadRow);
            RemoveWorkloadCommand = new RelayCommand(ExecuteRemoveWorkload);
            GenerateScheduleCommand = new RelayCommand(ExecuteGenerateSchedule, CanExecuteGenerateSchedule);
            ClearAllCommand = new RelayCommand(ExecuteClearAll);
            SaveScheduleCommand = new RelayCommand(ExecuteSaveSchedule, CanExecuteSaveSchedule);
        }

        // Властивості для прив'язки даних
        public ObservableCollection<string> Teachers { get; }
        public ObservableCollection<string> Subjects { get; }
        public ObservableCollection<string> Groups { get; }
        public ObservableCollection<string> Buildings { get; }
        public ObservableCollection<WorkloadItem> TeacherWorkload { get; }
        public ObservableCollection<ScheduleItem> GeneratedSchedule { get; }

        public string SelectedTeacher
        {
            get => _selectedTeacher;
            set 
            { 
                SetProperty(ref _selectedTeacher, value);
                AddTeacherCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedBuilding
        {
            get => _selectedBuilding;
            set => SetProperty(ref _selectedBuilding, value);
        }

        public bool IsGenerating
        {
            get => _isGenerating;
            set 
            { 
                SetProperty(ref _isGenerating, value);
                GenerateScheduleCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasGeneratedSchedule
        {
            get => _hasGeneratedSchedule;
            set 
            { 
                SetProperty(ref _hasGeneratedSchedule, value);
                SaveScheduleCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AddTeacherCommand { get; }
        public RelayCommand AddWorkloadRowCommand { get; }
        public RelayCommand RemoveWorkloadCommand { get; }
        public RelayCommand GenerateScheduleCommand { get; }
        public RelayCommand ClearAllCommand { get; }
        public RelayCommand SaveScheduleCommand { get; }

        private bool CanExecuteAddTeacher(object parameter) => !string.IsNullOrEmpty(SelectedTeacher);
        private bool CanExecuteAddWorkloadRow(object parameter) => TeacherWorkload.Any();
        private bool CanExecuteGenerateSchedule(object parameter) => !IsGenerating && TeacherWorkload.Any();
        private bool CanExecuteSaveSchedule(object parameter) => HasGeneratedSchedule && !IsGenerating;

        private void ExecuteAddTeacher(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedTeacher) && 
                !TeacherWorkload.Any(w => w.Teacher == SelectedTeacher))
            {
                TeacherWorkload.Add(new WorkloadItem { Teacher = SelectedTeacher });
                AddWorkloadRowCommand.RaiseCanExecuteChanged();
                GenerateScheduleCommand.RaiseCanExecuteChanged();
                Debug.WriteLine($"Додано викладача: {SelectedTeacher}");
            }
        }

        private void ExecuteAddWorkloadRow(object parameter)
        {
            if (TeacherWorkload.Any())
            {
                var lastTeacher = TeacherWorkload.Last();
                TeacherWorkload.Add(new WorkloadItem { Teacher = lastTeacher.Teacher });
                Debug.WriteLine($"Додано рядок навантаження для: {lastTeacher.Teacher}");
            }
        }

        private void ExecuteRemoveWorkload(object parameter)
        {
            if (parameter is WorkloadItem workload)
            {
                TeacherWorkload.Remove(workload);
                AddWorkloadRowCommand.RaiseCanExecuteChanged();
                GenerateScheduleCommand.RaiseCanExecuteChanged();
                Debug.WriteLine($"Видалено навантаження: {workload.Teacher} - {workload.Subject}");
            }
        }

        private async void ExecuteGenerateSchedule(object parameter)
        {
            try
            {
                IsGenerating = true;
                HasGeneratedSchedule = false;
                GeneratedSchedule.Clear();

                if (!ValidateWorkloadData())
                {
                    Debug.WriteLine("Помилка: Некоректні дані навантаження");
                    return;
                }

                await Task.Delay(2000);

                GenerateSampleSchedule();

                HasGeneratedSchedule = true;
                Debug.WriteLine($"Розклад згенеровано успішно. Занять: {GeneratedSchedule.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка генерації розкладу: {ex.Message}");
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private void ExecuteClearAll(object parameter)
        {
            TeacherWorkload.Clear();
            GeneratedSchedule.Clear();
            SelectedTeacher = null;
            HasGeneratedSchedule = false;
            AddWorkloadRowCommand.RaiseCanExecuteChanged();
            GenerateScheduleCommand.RaiseCanExecuteChanged();
            Debug.WriteLine("Всі дані очищено");
        }

        private void ExecuteSaveSchedule(object parameter)
        {
            Debug.WriteLine($"Розклад збережено. Занять: {GeneratedSchedule.Count}");
            
            HasGeneratedSchedule = false;
            Debug.WriteLine("Розклад опубліковано для студентів та викладачів");
        }

        private bool ValidateWorkloadData()
        {
            foreach (var workload in TeacherWorkload)
            {
                if (string.IsNullOrEmpty(workload.Teacher) ||
                    string.IsNullOrEmpty(workload.Subject) ||
                    string.IsNullOrEmpty(workload.Group) ||
                    workload.Hours <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void GenerateSampleSchedule()
        {
            var days = new[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };
            var times = new[] { "09:00-10:30", "10:45-12:15", "13:00-14:30", "14:45-16:15" };
            var classrooms = new[] { "101-A", "201-B", "301-C", "102-A", "202-B" };

            var random = new Random();

            foreach (var workload in TeacherWorkload.Take(5))
            {
                var day = days[random.Next(days.Length)];
                var time = times[random.Next(times.Length)];
                var classroom = classrooms[random.Next(classrooms.Length)];

                GeneratedSchedule.Add(new ScheduleItem
                {
                    Day = day,
                    Time = time,
                    Teacher = workload.Teacher,
                    Subject = workload.Subject,
                    Group = workload.Group,
                    Classroom = classroom,
                    Type = GetLessonType(workload.Subject)
                });
            }
        }

        private string GetLessonType(string subject)
        {
            return subject switch
            {
                "Програмування" => "Лабораторна",
                "Бази даних" => "Практика",
                _ => "Лекція"
            };
        }
    }

    public class WorkloadItem : ViewModelBase
    {
        private string _teacher = string.Empty;
        private string _subject = string.Empty;
        private string _group = string.Empty;
        private int _hours;
        private string _classroomRequirements = "Стандартна";

        public string Teacher
        {
            get => _teacher;
            set => SetProperty(ref _teacher, value);
        }

        public string Subject
        {
            get => _subject;
            set => SetProperty(ref _subject, value);
        }

        public string Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }

        public int Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        public string ClassroomRequirements
        {
            get => _classroomRequirements;
            set => SetProperty(ref _classroomRequirements, value);
        }
    }

    public class ScheduleItem : ViewModelBase
    {
        public string Day { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}