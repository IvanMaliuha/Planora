using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Planora.ViewModels.Base; 

namespace Planora.UI
{
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? data)
        {
            if (data is null)
                return null;

            // Отримуємо повне ім'я: Planora.ViewModels.ViewModels.LoginViewModel
            var name = data.GetType().FullName!
                // 1. Міняємо папку: Planora.ViewModels.ViewModels -> Planora.Views
                .Replace("ViewModels.ViewModels", "Views")
                // 2. Міняємо назву файлу: LoginViewModel -> LoginView
                .Replace("ViewModel", "View");

            // Тепер шукаємо тип: Planora.Views.LoginView
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}