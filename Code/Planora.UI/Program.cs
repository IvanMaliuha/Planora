using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Serilog; // 👇 Додали

namespace Planora.UI
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // 👇 1. Налаштування Логера
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // Писати в консоль
                .WriteTo.File("logs/planora-log.txt", rollingInterval: RollingInterval.Day) // Писати у файл (новий щодня)
                .CreateLogger();

            try
            {
                Log.Information("🚀 Програма запускається...");
                
                BuildAvaloniaApp()
                    .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "🔥 Критична помилка! Програма впала.");
            }
            finally
            {
                Log.Information("🛑 Програма завершила роботу.");
                Log.CloseAndFlush();
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}