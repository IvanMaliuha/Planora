using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;

namespace Planora.ViewModels.ViewModels
{
    public class AdminDashboardViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;
        
        public event Action? OnLogout;

        public AdminDashboardViewModel()
        {
            LogoutCommand = new RelayCommand(_ => OnLogout?.Invoke());
            NavigateCommand = new RelayCommand(Navigate);

            CurrentPage = new ClassroomManagementViewModel();
        }

        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public RelayCommand LogoutCommand { get; }
        public RelayCommand NavigateCommand { get; }

        private void Navigate(object parameter)
        {
            if (parameter is string destination)
            {
                switch (destination)
                {
                    case "Classrooms":
                        CurrentPage = new ClassroomManagementViewModel();
                        break;
                    case "Teachers":
                        CurrentPage = new TeacherManagementViewModel();
                        break;
                    case "Groups":
                        CurrentPage = new GroupManagementViewModel();
                        break;
                    case "Subjects": // 👇 Нова вкладка
                        CurrentPage = new SubjectManagementViewModel();
                        break;
                    case "Schedule":
                        var scheduleVm = new ScheduleGenerationViewModel();
                        // 👇 КОЛИ ЮЗЕР ТИСНЕ "ПЕРЕГЛЯНУТИ", ВІДКРИВАЄМО РОЗКЛАД
                        scheduleVm.OnViewResult += () => 
                        {
                            // Відкриваємо звичайний перегляд розкладу (можна той самий, що у студента)
                            CurrentPage = new ScheduleViewModel(); 
                        };
                        CurrentPage = scheduleVm;
                        break;
                }
            }
        }
    }
}