using System;
using System.ComponentModel;
using System.Windows.Input; // 👈 1. Цього не вистачало (це простір імен для команд)

namespace Planora.ViewModels.Commands
{
    // 👇 2. Додаємо ": ICommand" — тепер система знає, що це команда
    public class RelayCommand : ICommand 
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Цей метод перевіряє, чи можна натиснути кнопку
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        // Цей метод виконує дію
        public void Execute(object parameter) => _execute(parameter);

        // Ця подія каже кнопці: "Перевір, чи можна мене натиснути зараз"
        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}