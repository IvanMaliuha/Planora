using Avalonia.Controls;
using Avalonia.Input;

namespace Planora.Views
{
    public partial class TeacherTrackingView : UserControl
    {
        public TeacherTrackingView()
        {
            InitializeComponent();
        }

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
