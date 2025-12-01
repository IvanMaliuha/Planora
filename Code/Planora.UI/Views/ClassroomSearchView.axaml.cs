using Avalonia.Controls;
using Avalonia.Input; // Потрібно для KeyEventArgs

namespace Planora.Views
{
    public partial class ClassroomSearchView : UserControl
    {
        public ClassroomSearchView()
        {
            InitializeComponent();
        }

        // Обробка натискання на фон (щоб зняти фокус) - ми це залишили з минулого разу,
        // якщо ви не прибрали PointerPressed у XAML, це теж працюватиме
        private void OnBackgroundClicked(object? sender, PointerPressedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            topLevel?.FocusManager?.ClearFocus();
        }

        // 👇 Обробка Enter в полі пошуку
        private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var topLevel = TopLevel.GetTopLevel(this);
                topLevel?.FocusManager?.ClearFocus();
            }
        }
    }
}