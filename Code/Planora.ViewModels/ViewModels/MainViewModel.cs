using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;

namespace Planora.ViewModels.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private string _title = "Planora - Система управління розкладом";

        public MainViewModel()
        {
            // Замість простого створення, ми викликаємо метод, який налаштовує переходи
            ShowLogin();
        }

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // Цей метод показує екран входу і "слухає", чи пройшов вхід успішно
        public void ShowLogin()
        {
            var loginVm = new LoginViewModel();
            
            // 👇 ТЕПЕР МИ ОТРИМУЄМО ЛОГІН (username)
            loginVm.OnLoginSuccess += (username) => 
            {
                if (username.ToLower() == "admin")
                {
                    ShowAdminDashboard();
                }
                else if (username.ToLower() == "teacher") // 👇 ДОДАЛИ ВИКЛАДАЧА
                {
                    ShowTeacherDashboard();
                }
                else
                {
                    ShowStudentDashboard();
                }
            };
            
            CurrentViewModel = loginVm;
        }

        public void ShowStudentDashboard()
        {
            var dashboardVm = new DashboardViewModel();
            dashboardVm.OnLogout += () => ShowLogin();
            CurrentViewModel = dashboardVm;
        }

        public void ShowAdminDashboard()
        {
            var adminVm = new AdminDashboardViewModel();
            adminVm.OnLogout += () => ShowLogin();
            CurrentViewModel = adminVm;
        }

        public void ShowTeacherDashboard()
        {
            var teacherVm = new TeacherDashboardViewModel();
            teacherVm.OnLogout += () => ShowLogin();
            CurrentViewModel = teacherVm;
        }

        // Цей метод перемикає екран на головний (Dashboard)
        public void ShowDashboard()
        {
            CurrentViewModel = new DashboardViewModel();
            var dashboardVm = new DashboardViewModel();
            dashboardVm.OnLogout += () => ShowLogin();
            
            CurrentViewModel = dashboardVm;
        }
    }
}