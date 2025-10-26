using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PlanoraConsoleApp
{
    // Модель для читання Workload.csv (повинна мати ключі)
    public class WorkloadRecord
    {
        // КЛЮЧІ (UserId = TeacherId)
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public int Duration { get; set; } // Загальна кількість годин

        // Деталі для логіки та звітності
        public string TeacherName { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public string ClassroomNumber { get; set; } = "";
        public string ClassroomBuilding { get; set; } = ""; // Ключ для унікальності
        
        // Ігноруємо непотрібні поля з CSV для простоти читання
        public string SubjectType { get; set; } = "";
        public int ClassroomCapacity { get; set; }
        public bool HasProjector { get; set; }
        public bool HasComputers { get; set; }
    }
    
    // Клас ScheduleGenerator тепер приймає DbContext для збереження
    public class ScheduleGenerator
    {
        private const int DaysPerWeek = 5;
        private const int SlotsPerDay = 8; 

        // 80 хвилин пара. Перерва 20 хвилин. Загальний інтервал 100 хвилин.
        // Час початку пар
        private static readonly TimeOnly[] StartTimes = 
        {
            new TimeOnly(8, 30), new TimeOnly(10, 10), new TimeOnly(11, 50), new TimeOnly(13, 30),
            new TimeOnly(15, 10), new TimeOnly(16, 50), new TimeOnly(18, 30), new TimeOnly(20, 10)
        };
        private static readonly TimeSpan PairDuration = TimeSpan.FromMinutes(80); // Тривалість пари

        private readonly PlanoraDbContext _context;

        public ScheduleGenerator(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task GenerateAsync(string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"Файл {csvFilePath} не знайдено!");
                return;
            }

            List<WorkloadRecord> workloads;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null
            };

            // 1. Читання Workload з CSV
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                workloads = csv.GetRecords<WorkloadRecord>().ToList();
            }

            // 2. Підготовка: Мапинг ClassroomNumber + Building -> ClassroomId
            // ВИПРАВЛЕННЯ: Використовуємо складений ключ "Номер|Корпус" для уникнення помилки дублювання
            var classroomMap = await _context.Classrooms
                .ToDictionaryAsync(c => $"{c.Number}|{c.Building}", c => c.Id);
            
            // Видаляємо всі попередні записи розкладу
            await _context.Schedules.ExecuteDeleteAsync();
            
            var scheduleRecords = new List<Schedule>();
            
            // Матриці зайнятості
            var teacherOccupied = new Dictionary<int, bool[,]>();
            var groupOccupied = new Dictionary<int, bool[,]>();

            // 3. Логіка планування
            foreach (var w in workloads.OrderByDescending(w => w.Duration)) // Плануємо спочатку велике навантаження
            {
                if (!teacherOccupied.ContainsKey(w.UserId))
                    teacherOccupied[w.UserId] = new bool[DaysPerWeek, SlotsPerDay];

                if (!groupOccupied.ContainsKey(w.GroupId))
                    groupOccupied[w.GroupId] = new bool[DaysPerWeek, SlotsPerDay];
                
                // Кількість пар, які потрібно запланувати (припускаємо 1 пара = 2 години)
                int hoursPerPair = 2; 
                int pairsToSchedule = (int)Math.Ceiling((double)w.Duration / hoursPerPair); 

                // Отримуємо ClassroomId
                // ВИПРАВЛЕННЯ: Формуємо той самий складений ключ для пошуку
                string searchKey = $"{w.ClassroomNumber}|{w.ClassroomBuilding}";
                
                if (!classroomMap.TryGetValue(searchKey, out int classroomId))
                {
                    Console.WriteLine($"❌ Аудиторія {w.ClassroomNumber} (Корпус {w.ClassroomBuilding}) для групи {w.GroupName} не знайдена в БД. Пропуск.");
                    continue;
                }
                
                // Планування по днях і слотах
                for (int day = 0; day < DaysPerWeek && pairsToSchedule > 0; day++)
                {
                    for (int slot = 0; slot < SlotsPerDay && pairsToSchedule > 0; slot++)
                    {
                        // Перевірка на конфлікт
                        if (!teacherOccupied[w.UserId][day, slot] &&
                            !groupOccupied[w.GroupId][day, slot])
                        {
                            var startTime = StartTimes[slot];
                            var endTime = startTime.Add(PairDuration);

                            scheduleRecords.Add(new Schedule
                            {
                                UserId = w.UserId,
                                SubjectId = w.SubjectId,
                                GroupId = w.GroupId,
                                ClassroomId = classroomId,
                                DayOfWeek = day + 1, // 1 = Понеділок
                                StartTime = startTime,
                                EndTime = endTime,
                                WeekType = "both",
                            });

                            teacherOccupied[w.UserId][day, slot] = true;
                            groupOccupied[w.GroupId][day, slot] = true;
                            pairsToSchedule--;
                        }
                    }
                }

                if (pairsToSchedule > 0)
                {
                    Console.WriteLine($"⚠ Не вистачило місця для {pairsToSchedule * hoursPerPair} годин предмету {w.SubjectName} групи {w.GroupName}");
                }
            }

            // 4. Збереження розкладу у БД
            _context.Schedules.AddRange(scheduleRecords);
            await _context.SaveChangesAsync();

            Console.WriteLine($"\n✅ Розклад з {scheduleRecords.Count} пар успішно згенеровано та збережено у базі даних (таблиця Schedules).");
        }
    }
}
