using Planora.ViewModels.Base;
using Planora.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System; // 👇 Потрібно для Action

namespace Planora.ViewModels.ViewModels
{
    public class ScheduleGenerationViewModel : ViewModelBase
    {
        private bool _isGenerating;
        private int _progress;
        private string _statusMessage = "Очікування запуску...";
        private bool _isCompleted;

        // 👇 1. ПОДІЯ ПЕРЕХОДУ
        public event Action? OnViewResult;

        public ScheduleGenerationViewModel()
        {
            GenerateCommand = new RelayCommand(ExecuteGenerate, _ => !IsGenerating);
            
            // 👇 2. КОМАНДА ДЛЯ КНОПКИ
            ViewResultCommand = new RelayCommand(_ => OnViewResult?.Invoke());
            
            Parameters = new GenerationParameters();
        }

        public GenerationParameters Parameters { get; }

        public bool IsGenerating
        {
            get => _isGenerating;
            set 
            { 
                SetProperty(ref _isGenerating, value);
                GenerateCommand.RaiseCanExecuteChanged();
            }
        }

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        public RelayCommand GenerateCommand { get; }
        
        // 👇 3. ВЛАСТИВІСТЬ КОМАНДИ
        public RelayCommand ViewResultCommand { get; }

        private async void ExecuteGenerate(object parameter)
        {
            IsGenerating = true;
            IsCompleted = false;
            Progress = 0;
            StatusMessage = "Ініціалізація алгоритму...";

            try
            {
                // Імітація роботи
                for (int i = 0; i <= 100; i += 10)
                {
                    Progress = i;
                    
                    if (i < 30) StatusMessage = "Аналіз навантаження викладачів...";
                    else if (i < 60) StatusMessage = "Розподіл аудиторій...";
                    else if (i < 90) StatusMessage = "Оптимізація вікон у розкладі...";
                    else StatusMessage = "Збереження результатів...";

                    await Task.Delay(200); 
                }

                StatusMessage = "Розклад успішно згенеровано!";
                IsCompleted = true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка: {ex.Message}";
            }
            finally
            {
                IsGenerating = false;
            }
        }
    }

    public class GenerationParameters : ViewModelBase
    {
        private string _semester = "Осінній 2025";
        private bool _optimizeWindows = true;
        private bool _considerWishes = true;

        public string Semester { get => _semester; set => SetProperty(ref _semester, value); }
        public bool OptimizeWindows { get => _optimizeWindows; set => SetProperty(ref _optimizeWindows, value); }
        public bool ConsiderWishes { get => _considerWishes; set => SetProperty(ref _considerWishes, value); }
    }
}
