using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;

namespace Planora.ViewModels.ViewModels
{
    public class TeacherDashboardViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;
        private string _userName = "Петрова М.В."; // Заглушка
        private string _userRole = "Викладач";

        public event Action? OnLogout;

        public TeacherDashboardViewModel()
        {
            LogoutCommand = new RelayCommand(_ => OnLogout?.Invoke());
            NavigateCommand = new RelayCommand(Navigate);

            // Стартова сторінка - Розклад викладача
            CurrentPage = new TeacherScheduleViewModel();
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
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
                    case "Schedule":
                        CurrentPage = new TeacherScheduleViewModel();
                        break;
                    case "Classrooms":
                        CurrentPage = new ClassroomSearchViewModel(); // Спільний модуль
                        break;
                    case "Profile":
                        CurrentPage = new ProfileViewModel(); // Спільний модуль
                        break;
                }
            }
        }
    }
}