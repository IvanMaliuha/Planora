using Avalonia.Controls;
using Avalonia.Input; // 👇 Потрібно для роботи з клавішами

namespace Planora.Views
{
    public partial class TeacherTrackingView : UserControl
    {
        public TeacherTrackingView()
        {
            InitializeComponent();
        }

        // 👇 Метод, який спрацьовує при натисканні клавіші в полі пошуку
        private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Знаходимо головне вікно і знімаємо фокус
                var topLevel = TopLevel.GetTopLevel(this);
                topLevel?.FocusManager?.ClearFocus();
            }
        }
    }
}