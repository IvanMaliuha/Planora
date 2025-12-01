using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System; // Для Action

namespace Planora.ViewModels.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;
        private string _userName = "Іван Малюга";
        private string _userRole = "Студент";

        // 👇 Подія виходу
        public event Action? OnLogout;

        public DashboardViewModel()
        {
            // 👇 При натисканні кнопки викликаємо подію
            LogoutCommand = new RelayCommand(_ => OnLogout?.Invoke());

            // 👇 Використовуємо нове ім'я класу (ScheduleViewModel)
            CurrentPage = new ScheduleViewModel();
            
            NavigateCommand = new RelayCommand(Navigate);
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
                        // 👇 Виправлене ім'я
                        CurrentPage = new ScheduleViewModel(); 
                        break;
                    case "Classrooms":
                        CurrentPage = new ClassroomSearchViewModel();
                        break;
                    case "Teachers":
                        CurrentPage = new TeacherTrackingViewModel(); 
                        break;
                    case "Profile":
                        // 👇 Тепер це працює
                        CurrentPage = new ProfileViewModel(); 
                        break;
                }
            }
        }
    }
}