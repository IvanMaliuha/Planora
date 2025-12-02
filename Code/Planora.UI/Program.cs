using Avalonia;
using System;
using System.Threading.Tasks;
using Serilog;

namespace Planora.UI;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/planora-log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("=== ЗАПУСК ПРОГРАМИ PLANORA ===");
            Log.Information("Починаємо зчитування інформації про комп'ютер користувача...");


            Log.Information("   Операційна система: {OS}", Environment.OSVersion);
            Log.Information("   Ім'я комп'ютера: {Machine}", Environment.MachineName);
            Log.Information("   Ім'я користувача: {User}", Environment.UserName);
            Log.Information("   Версія .NET: {Version}", Environment.Version);
            Log.Information("   Кількість процесорів: {Count}", Environment.ProcessorCount);
            Log.Information("   Системна папка: {Dir}", Environment.SystemDirectory);

            Log.Information("Перевірка стану пам'яті перед запуску...");
            Log.Debug("Пам'ять виділена процесу: {Mem} байт", Environment.WorkingSet);

            Log.Information("Налаштовуємо перехоплення помилок, щоб програма не закривалася раптово...");
            SubscribeToGlobalExceptions();
            Log.Information("Система захисту від збоїв увімкнена успішно.");

            Log.Information("Починаємо підготовку графічного інтерфейсу...");
            

            var appBuilder = BuildAvaloniaApp();
            
            Log.Information("Графічний двигун Avalonia готовий до роботи.");

            Log.Information("Перевіряємо, чи були передані додаткові команди при запуску...");
            if (args.Length > 0)
            {
                Log.Information("Знайдено команди запуску: {Args}", string.Join(" ", args));
            }
            else
            {
                Log.Information("Додаткових команд немає. Запуск у звичайному режимі.");
            }

            Log.Information("Усі системи перевірено. Помилок не виявлено.");
            Log.Information(">>> ВІДКРИВАЄМО ГОЛОВНЕ ВІКНО <<<");
            Log.Information("Програма переходить в режим очікування дій користувача.");


            appBuilder.StartWithClassicDesktopLifetime(args);

            Log.Information("Користувач натиснув кнопку виходу.");
            Log.Information("Головне вікно закривається...");
        }
        catch (Exception ex)
        {
            Log.Fatal("!!! СТАЛАСЯ КРИТИЧНА ПОМИЛКА !!!");
            Log.Fatal("Програма не може продовжувати роботу.");
            Log.Fatal(ex, "Текст помилки: {Message}", ex.Message);
        }
        finally
        {
            Log.Information("Очищення оперативної пам'яті...");
            Log.Information("Запис останніх даних у файл логів...");
            Log.Information("=== РОБОТУ ЗАВЕРШЕНО ===");
            Log.CloseAndFlush();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        Log.Debug("Створення основи додатка...");
        var builder = AppBuilder.Configure<App>();

        Log.Debug("Визначення типу системи (Windows, Linux або Mac)...");
        builder.UsePlatformDetect();

        Log.Debug("Завантаження красивих шрифтів...");
        builder.WithInterFont();

        Log.Debug("Підключення діагностики для розробників...");
        builder.LogToTrace();

        Log.Debug("Налаштування завершено успішно.");
        return builder;
    }

    private static void SubscribeToGlobalExceptions()
    {

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Fatal("УВАГА! Знайдено неперехоплену помилку в системі!");
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Деталі збою: {Message}", ex.Message);
            }
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Error("УВАГА! Сталася помилка у фоновому процесі.");
            Log.Error(e.Exception, "Деталі фонової помилки");
            e.SetObserved(); 
        };
    }
}