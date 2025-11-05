using Planora.ViewModels.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Planora.UI.View
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dc = this.DataContext;
            MessageBox.Show($"Loaded. DataContext = {(dc != null ? dc.GetType().FullName : "null")}");
            Debug.WriteLine($"Loaded. DataContext = {(dc != null ? dc.GetType().FullName : "null")}");
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginViewModel.ErrorMessage))
            {
                Dispatcher.Invoke(() =>
                {
/*                    MessageBox.Show($"ErrorMessage changed: {_viewModel.ErrorMessage}");
                    Debug.WriteLine($"ErrorMessage changed: {_viewModel.ErrorMessage}");*/

                    if (_viewModel.ErrorMessage == "Успішний вхід!")
                    {
/*                        Debug.WriteLine("Авторизація успішна — відкриваємо ScheduleView.");
*/                        var scheduleView = new ScheduleView();
                        scheduleView.Show();
                        this.Close();
                    }
                });
            }
        }

        private void txtPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pw = ((PasswordBox)sender).Password;
/*            Debug.WriteLine($"PasswordChanged fired, password length = {pw?.Length}");
*/            if (DataContext is LoginViewModel vm)
            {
                vm.Password = pw;
/*                Debug.WriteLine("Assigned vm.Password");
*/            }
            else
            {
/*                Debug.WriteLine("DataContext is not LoginViewModel in PasswordChanged");
*/            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("btnLogin_Click fired");
            /*MessageBox.Show("btnLogin_Click fired");*/
            if (_viewModel != null)
            {
                try
                {
                    _viewModel.LoginCommand.Execute(null);
                    /*Debug.WriteLine("Manual Execute(LoginCommand) called.");*/
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine($"Login Execute exception: {ex}");
                    MessageBox.Show($"Login Execute exception: {ex.Message}");
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            /*Debug.WriteLine("btnReset_Click fired");
            MessageBox.Show("btnReset_Click fired");*/
            if (_viewModel != null)
            {
                _viewModel.ResetCommand.Execute(null);
                /*Debug.WriteLine("Manual Execute(ResetCommand) called.");*/
                txtPass.Password = string.Empty; // очищає поле з кружечками
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
