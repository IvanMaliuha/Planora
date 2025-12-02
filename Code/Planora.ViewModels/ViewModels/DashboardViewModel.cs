using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System;
using System.Windows.Input;
using Serilog; // Логування

namespace Planora.ViewModels.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;
        private string _userName = "Іван Малюга";
        private string _userRole = "Студент";

        public event Action? OnLogout;

        public DashboardViewModel()
        {
            Log.Debug("Ініціалізація DashboardViewModel. Завантаження головного меню...");

            // Кнопка виходу
            LogoutCommand = new RelayCommand(_ => 
            {
                Log.Information("Користувач натиснув 'Вийти'. Завершення сесії.");
                OnLogout?.Invoke();
            });

            // Навігація
            NavigateCommand = new RelayCommand(Navigate);

            // Стартова сторінка
            _currentPage = new ScheduleViewModel();
            Log.Information("Завантажено стартову сторінку: Розклад.");
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
                Log.Information("Навігація: Користувач перейшов у розділ '{Section}'.", destination);

                switch (destination)
                {
                    case "Schedule":
                        CurrentPage = new ScheduleViewModel();
                        break;
                    case "Classrooms":
                        CurrentPage = new ClassroomSearchViewModel();
                        break;
                    case "Teachers":
                        CurrentPage = new TeacherManagementViewModel(); 
                        break;
                    case "Profile":
                        CurrentPage = new ProfileViewModel();
                        break;
                    default:
                        Log.Warning("Спроба переходу на невідому сторінку: {Destination}", destination);
                        break;
                }
            }
        }
    }
}