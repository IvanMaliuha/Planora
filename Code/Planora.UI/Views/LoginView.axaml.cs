using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace Planora.Views
{
    public partial class LoginView : UserControl
    {
        private readonly Random _random = new Random();
        private bool _isViewLoaded = false;

        public LoginView()
        {
            InitializeComponent();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _isViewLoaded = true;

            // Запускаємо два незалежних потоки снігу
            StartWhiteSnow(); // Густий білий снігопад
            StartBlueSnow();  // Фоновий синій сніг
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _isViewLoaded = false; // Зупиняємо цикли при закритті
        }

        // --- ЦИКЛ 1: Білий сніг (Густий снігопад) ---
        private async void StartWhiteSnow()
        {
            var canvas = this.FindControl<Canvas>("DynamicSnowCanvas");
            if (canvas == null) return;

            while (_isViewLoaded)
            {
                double maxWidth = this.Bounds.Width > 0 ? this.Bounds.Width : 2000;

                var size = _random.Next(3, 9); // Трішки варіюємо розмір
                var snowFlake = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = Brushes.White,
                    Opacity = _random.NextDouble() * 0.4 + 0.6,
                    [Canvas.TopProperty] = -50.0,
                    [Canvas.LeftProperty] = _random.NextDouble() * maxWidth
                };

                snowFlake.Classes.Add("WhiteFallingSnow");
                canvas.Children.Add(snowFlake);

                DispatcherTimer.RunOnce(() =>
                {
                    if (canvas.Children.Contains(snowFlake)) canvas.Children.Remove(snowFlake);
                }, TimeSpan.FromSeconds(5.5));

                // 🔥 ЗМІНА ТУТ: Дуже мала затримка (5-50 мс) = багато снігу
                await Task.Delay(_random.Next(5, 50));
            }
        }

        // --- ЦИКЛ 2: Синій сніг (Фоновий, спокійний) ---
        private async void StartBlueSnow()
        {
            var canvas = this.FindControl<Canvas>("BackgroundSnowCanvas");
            if (canvas == null) return;

            // Заповнюємо екран на старті, щоб не чекати
            PrePopulateBlueSnow(canvas);

            while (_isViewLoaded)
            {
                double maxWidth = this.Bounds.Width > 0 ? this.Bounds.Width : 2000;

                var size = _random.Next(4, 10);
                var snowFlake = new Ellipse
                {
                    Width = size,
                    Height = size,
                    Fill = _random.Next(0, 2) == 0 ? 
                           new SolidColorBrush(Color.Parse("#93C5FD")) : 
                           new SolidColorBrush(Color.Parse("#BFDBFE")),
                    Opacity = _random.NextDouble() * 0.5 + 0.3,
                    [Canvas.TopProperty] = -50.0,
                    [Canvas.LeftProperty] = _random.NextDouble() * maxWidth
                };

                snowFlake.Classes.Add("BlueFallingSnow");
                canvas.Children.Add(snowFlake);

                DispatcherTimer.RunOnce(() =>
                {
                    if (canvas.Children.Contains(snowFlake)) canvas.Children.Remove(snowFlake);
                }, TimeSpan.FromSeconds(12.5));

                // Синій падає рідше, ніж білий
                await Task.Delay(_random.Next(100, 300));
            }
        }

        private void PrePopulateBlueSnow(Canvas canvas)
        {
             var width = 3000; var height = 1500;
             for (int i = 0; i < 40; i++) 
             {
                var size = _random.Next(4, 10);
                var snow = new Ellipse
                {
                    Width = size, Height = size,
                    Fill = new SolidColorBrush(Color.Parse("#93C5FD")),
                    Opacity = 0.5,
                    [Canvas.LeftProperty] = _random.NextDouble() * width,
                    [Canvas.TopProperty] = _random.NextDouble() * height
                };
                snow.Classes.Add("BlueFallingSnow"); 
                canvas.Children.Add(snow);
             }
        }
    }
}
