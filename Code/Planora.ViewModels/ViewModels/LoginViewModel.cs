using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Diagnostics;

namespace Planora.ViewModels.ViewModels
{
  public class LoginViewModel : ViewModelBase
  {
    private string _login = string.Empty;
    private string _password = string.Empty;
    private bool _rememberMe;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _failedAttempts;
    private DateTime? _lastFailedAttempt;
    private bool _isLocked;

    public LoginViewModel()
    {
      LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
      ResetCommand = new RelayCommand(ExecuteReset);

      PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(Login) || e.PropertyName == nameof(Password) || e.PropertyName == nameof(IsLoading))
        {
          LoginCommand.RaiseCanExecuteChanged();
        }
      };
    }

    public string Login
    {
      get => _login;
      set => SetProperty(ref _login, value);
    }

    public string Password
    {
      get => _password;
      set => SetProperty(ref _password, value);
    }

    public bool RememberMe
    {
      get => _rememberMe;
      set => SetProperty(ref _rememberMe, value);
    }

    public string ErrorMessage
    {
      get => _errorMessage;
      set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
      get => _isLoading;
      set => SetProperty(ref _isLoading, value);
    }

    public bool IsLocked
    {
      get => _isLocked;
      set => SetProperty(ref _isLocked, value);
    }

    public RelayCommand LoginCommand { get; }
    public RelayCommand ResetCommand { get; }

    private bool CanExecuteLogin(object parameter)
    {
      return !string.IsNullOrWhiteSpace(Login) &&
             !string.IsNullOrWhiteSpace(Password) &&
             !IsLoading &&
             !IsLocked;
    }

    private async void ExecuteLogin(object parameter)
    {
      if (IsLocked)
      {
        ErrorMessage = "Обліковий запис тимчасово заблоковано. Спробуйте пізніше.";
        return;
      }

      try
      {
        IsLoading = true;
        ErrorMessage = string.Empty;


        var loginValidation = ValidateLogin(Login);
        if (!loginValidation.IsValid)
        {
          ErrorMessage = loginValidation.ErrorMessage;
          return;
        }


        var passwordValidation = ValidatePassword(Password);
        if (!passwordValidation.IsValid)
        {
          ErrorMessage = passwordValidation.ErrorMessage;
          return;
        }

        await Task.Delay(1500);

        bool isAuthenticated = SimulateAuthentication(Login, Password);

        if (isAuthenticated)
        {
          _failedAttempts = 0;
          ErrorMessage = "Успішний вхід!";
          Debug.WriteLine($"Успішний вхід для користувача: {Login}");

        }
        else
        {
          _failedAttempts++;
          _lastFailedAttempt = DateTime.Now;
          ErrorMessage = "Невірний логін або пароль. Спробуйте ще раз.";
          Debug.WriteLine($"Невдала спроба входу для: {Login}. Спроба #{_failedAttempts}");

          CheckAccountLock();
        }
      }
      catch (Exception ex)
      {
        ErrorMessage = "Сталася помилка під час входу. Спробуйте ще раз.";
        Debug.WriteLine($"Помилка авторизації: {ex.Message}");
      }
      finally
      {
        IsLoading = false;
      }
    }

    private void ExecuteReset(object parameter)
    {
      Login = string.Empty;
      Password = string.Empty;
      ErrorMessage = string.Empty;
      RememberMe = false;
    }

    private ValidationResult ValidateLogin(string login)
    {
      if (string.IsNullOrWhiteSpace(login))
        return ValidationResult.Failed("Логін не може бути порожнім");

      if (login.Length < 3)
        return ValidationResult.Failed("Логін повинен містити мінімум 3 символи");

      if (!System.Text.RegularExpressions.Regex.IsMatch(login, @"^[a-zA-Z0-9_\-\.@]+$"))
        return ValidationResult.Failed("Логін містить недозволені символи");

      return ValidationResult.Success;
    }

    private ValidationResult ValidatePassword(string password)
    {
      if (string.IsNullOrWhiteSpace(password))
        return ValidationResult.Failed("Пароль не може бути порожнім");

      if (password.Length < 8)
        return ValidationResult.Failed("Пароль повинен містити мінімум 8 символів");

      if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
        return ValidationResult.Failed("Пароль повинен містити великі та малі літери та цифри");

      return ValidationResult.Success;
    }

    private bool SimulateAuthentication(string login, string password)
    {
      // тестові
      var testUsers = new[]
      {
                new { Login = "admin", Password = "Admin123" },
                new { Login = "teacher", Password = "Teacher123" },
                new { Login = "student", Password = "Student123" }
            };

      return testUsers.Any(u => u.Login == login && u.Password == password);
    }

    private void CheckAccountLock()
    {
      if (_failedAttempts >= 3)
      {
        if (_lastFailedAttempt.HasValue &&
            DateTime.Now - _lastFailedAttempt.Value < TimeSpan.FromMinutes(5))
        {
          IsLocked = true;
          ErrorMessage = "Обліковий запис заблоковано на 5 хвилин через надто багато невдалих спроб.";
          Debug.WriteLine("Акаунт заблоковано на 5 хвилин");

          Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(_ =>
          {
            IsLocked = false;
            _failedAttempts = 0;
            ErrorMessage = "Обліковий запис розблоковано. Спробуйте знову.";
            Debug.WriteLine("Акаунт розблоковано");
          });
        }
        else
        {
          _failedAttempts = 1;
        }
      }
    }
  }

  public class ValidationResult
  {
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ValidationResult Success => new ValidationResult { IsValid = true };

    public static ValidationResult Failed(string message) => new ValidationResult
    {
      IsValid = false,
      ErrorMessage = message
    };
  }
}