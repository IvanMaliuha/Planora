using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using Planora.BLL.Services; 
using Serilog; 

namespace Planora.ViewModels.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // Подія успішного входу для перемикання вікон
        public event Action<string>? OnLoginSuccess;

        // --- Сервіси (Clean Code: логіка винесена з ViewModel) ---
        private readonly IAuthenticationService _authService;
        private readonly ICredentialService _credentialService;

        // --- Поля даних ---
        private string _login = string.Empty;
        private string _password = string.Empty;
        private bool _rememberMe;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        // --- Поля логіки блокування ---
        private bool _isLocked;
        private int _failedAttempts;
        private DateTime? _lastFailedAttempt;

        // Конструктор
        public LoginViewModel()
        {
            // Ініціалізація сервісів
            _authService = new AuthenticationService();
            _credentialService = new FileCredentialService();

            // Команди
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            ResetCommand = new RelayCommand(ExecuteReset);

            // Підписка на зміни властивостей для оновлення доступності кнопки
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Login) ||
                    e.PropertyName == nameof(Password) ||
                    e.PropertyName == nameof(IsLoading) ||
                    e.PropertyName == nameof(IsLocked))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }
            };

            // Завантаження збережених даних
            LoadSavedData();
            
            Log.Information("LoginViewModel ініціалізовано. Очікування дій користувача.");
        }

        // --- Властивості (Properties) ---
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

        // --- Логіка команд ---

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Login) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading &&
                   !IsLocked;
        }

        private async void ExecuteLogin(object parameter)
        {
            // Перевірка блокування перед початком
            if (CheckIfLocked()) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Log.Information("Спроба входу користувача: {Login}", Login);

                // 1. Валідація введених даних
                if (!ValidateInput())
                {
                    IsLoading = false;
                    return;
                }

                // Імітація затримки мережі
                await Task.Delay(1500);

                // 2. Виклик сервісу аутентифікації
                bool isAuthenticated = _authService.Authenticate(Login, Password);

                if (isAuthenticated)
                {
                    HandleSuccessLogin();
                }
                else
                {
                    HandleFailedLogin();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Сталася критична помилка під час входу.";
                // ЛОГУВАННЯ ПОМИЛКИ (Task 6)
                Log.Error(ex, "Критична помилка при спробі входу користувача {Login}", Login);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ExecuteReset(object parameter)
        {
            Log.Information("Користувач {Login} натиснув 'Скинути'. Поля очищено.", Login);
            Login = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
            RememberMe = false;
            
            // Видаляємо збережений файл через сервіс
            _credentialService.ClearCredentials();
        }

        // --- Приватні методи (Clean Code: деталі реалізації сховані) ---

        private void LoadSavedData()
        {
            try
            {
                var (savedLogin, remember) = _credentialService.LoadCredentials();
                if (remember)
                {
                    Login = savedLogin;
                    RememberMe = true;
                    Log.Debug("Автоматично підставлено логін із збереженого файлу.");
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Не вдалося завантажити збережені дані.");
            }
        }

        private bool ValidateInput()
        {
            var loginResult = ValidateLoginRule(Login);
            if (!loginResult.IsValid)
            {
                ErrorMessage = loginResult.ErrorMessage;
                Log.Warning("Невдала валідація логіна '{Login}': {Error}", Login, loginResult.ErrorMessage);
                return false;
            }

            var passResult = ValidatePasswordRule(Password);
            if (!passResult.IsValid)
            {
                ErrorMessage = passResult.ErrorMessage;
                Log.Warning("Невдала валідація пароля для користувача '{Login}': {Error}", Login, passResult.ErrorMessage);
                return false;
            }
            return true;
        }

        private void HandleSuccessLogin()
        {
            _failedAttempts = 0;
            ErrorMessage = "Успішний вхід!";
            Log.Information("Користувач {Login} успішно авторизувався.", Login);

            _credentialService.SaveCredentials(Login, RememberMe);
            OnLoginSuccess?.Invoke(Login);
        }

        private void HandleFailedLogin()
        {
            _failedAttempts++;
            _lastFailedAttempt = DateTime.Now;
            ErrorMessage = "Невірний логін або пароль. Спробуйте ще раз.";
            
            Log.Warning("Невдала спроба входу для: {Login}. Спроба #{Attempt}", Login, _failedAttempts);

            CheckAccountLockLogic();
        }

        // --- Логіка блокування (Account Lock) ---

        private bool CheckIfLocked()
        {
            if (IsLocked)
            {
                ErrorMessage = "Обліковий запис тимчасово заблоковано.";
                Log.Information("Спроба входу заблокованого користувача {Login}.", Login);
                return true;
            }
            return false;
        }

        private void CheckAccountLockLogic()
        {
            if (_failedAttempts >= 3)
            {
                if (_lastFailedAttempt.HasValue && DateTime.Now - _lastFailedAttempt.Value < TimeSpan.FromMinutes(5))
                {
                    LockAccount();
                }
                else
                {
                    // Якщо час пройшов, скидаємо лічильник
                    _failedAttempts = 1;
                    Log.Debug("Лічильник невдалих спроб скинуто по тайм-ауту.");
                }
            }
        }

        private void LockAccount()
        {
            IsLocked = true;
            ErrorMessage = "Обліковий запис заблоковано на 5 хвилин через надто багато невдалих спроб.";
            Log.Warning("АКАУНТ ЗАБЛОКОВАНО: Користувач {Login} перевищив ліміт спроб.", Login);

            // Розблокування через 5 хвилин
            Task.Delay(TimeSpan.FromMinutes(5)).ContinueWith(_ =>
            {
                IsLocked = false;
                _failedAttempts = 0;
                ErrorMessage = "Обліковий запис розблоковано. Спробуйте знову.";
                Log.Information("Акаунт {Login} автоматично розблоковано.", Login);
            }); // Note: UI updates should technically be on UI thread, but properties handle notify logic.
        }

        // --- Правила валідації (Static Validation Rules) ---

        private static ValidationResult ValidateLoginRule(string login)
        {
            if (string.IsNullOrWhiteSpace(login)) 
                return ValidationResult.Failed("Логін не може бути порожнім");
            
            if (login.Length < 3) 
                return ValidationResult.Failed("Логін повинен містити мінімум 3 символи");
            
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_\-\.@]+$")) 
                return ValidationResult.Failed("Логін містить недозволені символи");

            return ValidationResult.Success;
        }

        private static ValidationResult ValidatePasswordRule(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) 
                return ValidationResult.Failed("Пароль не може бути порожнім");
            
            if (password.Length < 8) 
                return ValidationResult.Failed("Пароль повинен містити мінімум 8 символів");
            
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")) 
                return ValidationResult.Failed("Пароль повинен містити великі та малі літери та цифри");

            return ValidationResult.Success;
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