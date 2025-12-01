using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Planora.DAL;
using Planora.BLL.Handlers.Commands.Teachers;
using Planora.BLL.Handlers.Commands.Groups;
using Planora.BLL.Handlers.Commands.Students;
using Planora.BLL.Handlers.Commands.Subjects;
using Planora.BLL.Handlers.Commands.Classrooms;
using Planora.BLL.Handlers.Commands.TeachingAssignments;
using Planora.BLL.Handlers.Commands.GroupDisciplineLists;
using Planora.BLL.Handlers.Commands.Workload;
using Planora.BLL.Services;
using MediatR;
using Planora.BLL.DTOs;
using System.Threading.Tasks;
using System.IO;
using PlanoraConsoleApp; 
using System;
using System.Linq;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Planora.BLL.DTOs.Queries; 
using Planora.BLL.Handlers.Queries.Classrooms;
using Planora.BLL.Handlers.Queries.Teachers; // 👈 НОВИЙ USING

// --- КОНФІГУРАЦІЯ ХОСТУ ---

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = "Host=localhost;Port=5432;Database=Planora_db;Username=postgres;Password=2025";

        services.AddDbContext<PlanoraDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        // Реєстрація MediatR з BLL асемблеї (автоматично знаходить команди та запити)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddTeacherCommand).Assembly));

        // Реєстрація сервісів
        services.AddScoped<IWorkloadService, WorkloadService>();
        
        // Реєструємо ScheduleGenerator
        services.AddScoped<ScheduleGenerator>(); 
        services.AddTransient<ConsoleAppRunner>();
    })
    .Build();

await host.StartAsync();
var runner = host.Services.GetRequiredService<ConsoleAppRunner>(); 
await runner.RunAsync();

// --- RUNNER КОНСОЛЬНОГО ЗАСТОСУНКУ ---

public class ConsoleAppRunner
{
    private readonly IMediator _mediator;
    private readonly ScheduleGenerator _generator; 
    private readonly PlanoraDbContext _context;
    private readonly IWorkloadService _workloadService;

    public ConsoleAppRunner(IMediator mediator, PlanoraDbContext context, IWorkloadService workloadService, ScheduleGenerator generator)
    {
        _mediator = mediator;
        _context = context;
        _workloadService = workloadService;
        _generator = generator;
    }


    public async Task RunAsync()
    {
        Console.WriteLine("--- Консольний застосунок Planora: Режим Адміністратора ---");

        while (true)
        {
            Console.WriteLine("\nОберіть операцію:");
            Console.WriteLine("1. Додати Викладача");
            Console.WriteLine("2. Додати Групу");
            Console.WriteLine("3. Додати Студента");
            Console.WriteLine("4. Додати Предмет");
            Console.WriteLine("5. Додати Аудиторію");
            Console.WriteLine("6. Додати Призначення Викладання");
            Console.WriteLine("7. Додати Список Дисциплін Групи");
            Console.WriteLine("8. Згенерувати Workload (CSV)");
            Console.WriteLine("9. Згенерувати розклад з Workload.csv");
            Console.WriteLine("10. ЗНАЙТИ ВІЛЬНУ АУДИТОРІЮ"); 
            Console.WriteLine("11. ЗНАЙТИ ВИКЛАДАЧА"); // 👈 НОВА ОПЦІЯ
            Console.WriteLine("0. Вихід");
            Console.Write("Ваш вибір: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddTeacherInteractiveAsync();
                    break;
                case "2":
                    await AddGroupInteractiveAsync();
                    break;
                case "3":
                    await AddStudentInteractiveAsync();
                    break;
                case "4":
                    await AddSubjectInteractiveAsync();
                    break;
                case "5":
                    await AddClassroomInteractiveAsync();
                    break;
                case "6":
                    await AddTeachingAssignmentInteractiveAsync();
                    break;
                case "7":
                    await AddGroupDisciplineListInteractiveAsync();
                    break;
                case "8":
                    await GenerateWorkloadInteractiveAsync();
                    break;
                case "9":
                    await GenerateScheduleFromCsv(); 
                    break;
                case "10":
                    await FindFreeClassroomInteractiveAsync(); 
                    break;
                case "11": // 👈 НОВИЙ КЕЙС
                    await FindTeacherLocationInteractiveAsync();
                    break;

                case "0":
                    Console.WriteLine("👋 Вихід із програми.");
                    return;
                default:
                    Console.WriteLine("❌ Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    // --- Методи для додавання (ЗБЕРЕЖЕНІ) ---
    private async Task AddTeacherInteractiveAsync()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ ВИКЛАДАЧА ---");
        Console.Write("ПІБ: "); var fullName = Console.ReadLine();
        Console.Write("Email: "); var email = Console.ReadLine();
        Console.Write("Факультет: "); var faculty = Console.ReadLine();
        Console.Write("Посада: "); var position = Console.ReadLine();

        var command = new AddTeacherCommand(new AddTeacherDto { FullName = fullName, Email = email, Faculty = faculty, Position = position });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Викладача додано!" : "❌ Не вдалося додати викладача (Email вже існує).");
    }

    private async Task AddGroupInteractiveAsync()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ ГРУПИ ---");
        Console.Write("Назва групи: "); var name = Console.ReadLine();
        Console.Write("Факультет: "); var faculty = Console.ReadLine();
        Console.Write("Кількість студентів: "); int.TryParse(Console.ReadLine(), out int count);

        var command = new AddGroupCommand(new AddGroupDto { Name = name, Faculty = faculty, StudentCount = count });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Групу додано!" : "❌ Така група вже існує.");
    }

    private async Task AddStudentInteractiveAsync()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ СТУДЕНТА ---");
        Console.Write("ПІБ: "); var fullName = Console.ReadLine();
        Console.Write("Email: "); var email = Console.ReadLine();
        Console.Write("Факультет: "); var faculty = Console.ReadLine();
        Console.Write("Назва групи: "); var groupName = Console.ReadLine();

        var command = new AddStudentCommand(new AddStudentDto { FullName = fullName, Email = email, Faculty = faculty, GroupName = groupName });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Студента додано!" : "❌ Не вдалося додати (Email або група некоректні).");
    }

    private async Task AddSubjectInteractiveAsync()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ ПРЕДМЕТУ ---");
        Console.Write("Назва: "); var name = Console.ReadLine();
        Console.Write("Тип: "); var type = Console.ReadLine();
        Console.Write("Вимоги (необов’язково): "); var req = Console.ReadLine();
        Console.Write("Тривалість (годин): "); int.TryParse(Console.ReadLine(), out int duration);

        var command = new AddSubjectCommand(new AddSubjectDto { Name = name, Type = type, Requirements = req, Duration = duration });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Предмет додано!" : "❌ Такий предмет уже існує.");
    }

    private async Task AddClassroomInteractiveAsync()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ АУДИТОРІЇ ---");
        Console.Write("Номер: "); var number = Console.ReadLine();
        Console.Write("Корпус: "); var building = Console.ReadLine();
        Console.Write("Вмістимість: "); int.TryParse(Console.ReadLine(), out int capacity);
        Console.Write("Факультет: "); var faculty = Console.ReadLine();
        Console.Write("Комп’ютери (так/ні): "); bool hasPc = Console.ReadLine()?.Trim().ToLower() == "так";
        Console.Write("Проектор (так/ні): "); bool hasProj = Console.ReadLine()?.Trim().ToLower() == "так";

        var command = new AddClassroomCommand(new AddClassroomDto { Number = number, Building = building, Capacity = capacity, Faculty = faculty, HasComputers = hasPc, HasProjector = hasProj });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Аудиторію додано!" : "❌ Така аудиторія вже існує.");
    }

    private async Task AddTeachingAssignmentInteractiveAsync()
    {
        Console.WriteLine("\n--- ПРИЗНАЧЕННЯ ВИКЛАДАННЯ ---");
        Console.Write("ID Викладача: "); int.TryParse(Console.ReadLine(), out int userId);
        Console.Write("ID Предмета: "); int.TryParse(Console.ReadLine(), out int subjectId);
        Console.Write("Кількість годин: "); int.TryParse(Console.ReadLine(), out int hours);

        var command = new AddTeachingAssignmentCommand(new AddTeachingAssignmentDto { UserId = userId, SubjectId = subjectId, Hours = hours });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Призначення додано!" : "❌ Це призначення вже існує.");
    }

    private async Task AddGroupDisciplineListInteractiveAsync()
    {
        Console.WriteLine("\n--- ДОДАВАННЯ ДИСЦИПЛІНИ ДО ГРУПИ ---");
        Console.Write("ID Групи: "); int.TryParse(Console.ReadLine(), out int groupId);
        Console.Write("ID Предмета: "); int.TryParse(Console.ReadLine(), out int subjectId);
        Console.Write("Кількість годин: "); int.TryParse(Console.ReadLine(), out int hours);

        var command = new AddGroupDisciplineListCommand(new AddGroupDisciplineListDto { GroupId = groupId, SubjectId = subjectId, Hours = hours });
        var success = await _mediator.Send(command);
        Console.WriteLine(success ? "✅ Дисципліну додано до групи!" : "❌ Така дисципліна вже існує у групи.");
    }


    // --- Метод GenerateWorkloadInteractiveAsync (ЗБЕРЕЖЕНИЙ) ---
    private async Task GenerateWorkloadInteractiveAsync()
    {
        Console.WriteLine("\n--- ГЕНЕРАЦІЯ WORKLOAD (З КЛЮЧАМИ) ---");

        var command = new GenerateWorkloadCommand();
        var workload = await _mediator.Send(command);

        if (!workload.Any())
        {
            Console.WriteLine("❌ Workload порожній. Додайте призначення та дисципліни.");
            return;
        }

        Console.WriteLine($"\nЗгенеровано {workload.Count} записів Workload:\n");

        foreach (var item in workload)
        {
            Console.WriteLine($"Група: {item.GroupName} | Предмет: {item.SubjectName} ({item.SubjectType}) | Години: {item.Duration} | Викладач: {item.TeacherName} (ID: {item.UserId}) | Аудиторія: {item.ClassroomNumber}");
        }

        var csvPath = "Workload.csv";
        
        using (var writer = new StreamWriter(csvPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            // Записуємо заголовки, включаючи всі ID та деталі
            csv.WriteHeader<WorkloadDto>();
            csv.NextRecord();
            
            // Записуємо всі записи WorkloadDto
            csv.WriteRecords(workload);
        }
        
        Console.WriteLine($"\n✅ Workload збережено у файл {csvPath} з усіма полями DTO, включаючи ID.");
    }

    // --- МЕТОД ГЕНЕРАЦІЇ РОЗКЛАДУ З CSV (ВИПРАВЛЕНИЙ) ---
    private async Task GenerateScheduleFromCsv()
    {
        Console.WriteLine("\n--- ГЕНЕРАЦІЯ РОЗКЛАДУ ТА ЗБЕРЕЖЕННЯ У БД ---");

        var csvPath = "Workload.csv";
        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"❌ Файл {csvPath} не знайдено. Спершу згенеруйте Workload (опція 8).");
            return;
        }

        await _generator.GenerateAsync(csvPath);
    }
    
    // --- МЕТОД ПОШУКУ ВІЛЬНОЇ АУДИТОРІЇ ---
    private async Task FindFreeClassroomInteractiveAsync()
    {
        Console.WriteLine("\n--- ПОШУК ВІЛЬНОЇ АУДИТОРІЇ ---");

        // 1. Збір параметрів часу
        Console.Write("День тижня (1=ПН, 5=ПТ): "); 
        int dayOfWeek;
        if (!int.TryParse(Console.ReadLine(), out dayOfWeek) || dayOfWeek < 1 || dayOfWeek > 5)
        {
            Console.WriteLine("❌ Некоректний день тижня (має бути від 1 до 5).");
            return;
        }
        
        Console.WriteLine("Доступні часові слоти: 08:30, 10:10, 11:50, 13:30, 15:10, 16:50, 18:30, 20:10.");
        Console.Write("Час початку пари (напр., 08:30): "); 
        TimeOnly startTime;
        if (!TimeOnly.TryParse(Console.ReadLine(), out startTime))
        {
            Console.WriteLine("❌ Невірний формат часу.");
            return;
        }

        // 2. Збір вимог
        Console.Write("Корпус: "); var building = Console.ReadLine() ?? "";
        
        Console.Write("Потрібні комп'ютери (так/ні): "); 
        bool needsComputers = Console.ReadLine()?.Trim().ToLower() == "так";
        
        Console.Write("Потрібен проектор (так/ні): "); 
        bool needsProjector = Console.ReadLine()?.Trim().ToLower() == "так";
        
        Console.Write("Потрібна місткість (мін.): "); 
        int requiredCapacity;
        int.TryParse(Console.ReadLine(), out requiredCapacity);

        var searchData = new FindFreeClassroomQueryDto
        {
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            Building = building,
            NeedsComputers = needsComputers,
            NeedsProjector = needsProjector,
            RequiredCapacity = requiredCapacity
        };

        var query = new FindFreeClassroomQuery(searchData);
        
        // Відправляємо запит через MediatR
        var results = await _mediator.Send(query);

        Console.WriteLine("\n--- РЕЗУЛЬТАТИ ПОШУКУ ---");

        if (results.Any())
        {
            Console.WriteLine($"✅ Знайдено {results.Count} вільних аудиторій, що відповідають вимогам:");
            
            // Виводимо найкращий (найменш місткий) варіант першим
            var bestFit = results.First(); 
            
            Console.WriteLine($"РЕКОМЕНДОВАНО: [ID: {bestFit.ClassroomId}] Номер: {bestFit.Number}, Корпус: {bestFit.Building}, Місткість: {bestFit.Capacity}");
            
            if (results.Count > 1)
            {
                Console.WriteLine($"Інші варіанти (всього {results.Count}):");
                foreach (var r in results.Skip(1))
                {
                    Console.WriteLine($"  Номер: {r.Number}, Корпус: {r.Building}, Місткість: {r.Capacity}");
                }
            }
        }
        else
        {
            Console.WriteLine("❌ Не знайдено жодної вільної аудиторії з такими вимогами.");
        }
    }
    
    // 🛠️ НОВИЙ МЕТОД: ПОШУК ВИКЛАДАЧА (ОПЦІЯ 11)
    private async Task FindTeacherLocationInteractiveAsync()
    {
        Console.WriteLine("\n--- ПОШУК МІСЦЕЗНАХОДЖЕННЯ ВИКЛАДАЧА ---");

        Console.Write("ПІБ Викладача: "); 
        var fullName = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Поточний день (1=ПН, 5=ПТ): "); 
        int currentDay;
        if (!int.TryParse(Console.ReadLine(), out currentDay) || currentDay < 1 || currentDay > 5)
        {
            Console.WriteLine("❌ Некоректний день тижня. Використовуйте 1 (ПН) - 5 (ПТ).");
            return;
        }

        Console.Write("Поточний час (напр., 10:00): "); 
        TimeOnly currentTime;
        if (!TimeOnly.TryParse(Console.ReadLine(), out currentTime))
        {
            Console.WriteLine("❌ Невірний формат часу.");
            return;
        }

        var searchData = new FindTeacherLocationQueryDto
        {
            FullName = fullName,
            CurrentDayOfWeek = currentDay,
            CurrentTime = currentTime
        };

        var query = new FindTeacherLocationQuery(searchData);
        var result = await _mediator.Send(query);

        Console.WriteLine("\n--- РЕЗУЛЬТАТ ПОШУКУ ---");
        
        if (result.Message.StartsWith("❌"))
        {
            Console.WriteLine(result.Message);
            return;
        }

        Console.WriteLine($"Факультет викладача: {result.TeacherFaculty}");
        
        if (result.IsCurrentlyTeaching)
        {
            Console.WriteLine($"СИТУАЦІЯ: {result.Message}");
            Console.WriteLine($"  Пара триває: з {result.StartTime} до {result.EndTime}");
            Console.WriteLine($"  Місце: Корпус {result.Building}, Аудиторія {result.ClassroomNumber}");
        }
        else if (result.StartTime != default) // Якщо є наступна пара сьогодні
        {
            Console.WriteLine($"СИТУАЦІЯ: {result.Message}");
            Console.WriteLine($"  Наступна пара: з {result.StartTime} до {result.EndTime}");
            Console.WriteLine($"  Місце: Корпус {result.Building}, Аудиторія {result.ClassroomNumber}");
        }
        else
        {
            Console.WriteLine(result.Message);
        }
    }
}