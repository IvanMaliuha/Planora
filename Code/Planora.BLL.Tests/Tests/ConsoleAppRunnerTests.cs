using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Planora.DAL;
using Planora.DAL.Models;
using Planora.BLL.Handlers.Commands.Teachers;
using Planora.BLL.Handlers.Commands.Groups;
using Planora.BLL.Handlers.Commands.Students;
using Planora.BLL.Handlers.Commands.Subjects;
using Planora.BLL.Handlers.Commands.Classrooms;
using Planora.BLL.Handlers.Commands.TeachingAssignments;
using Planora.BLL.Handlers.Commands.GroupDisciplineLists;
using Planora.BLL.Handlers.Commands.Workload;
using Planora.BLL.Handlers.Queries.Classrooms;
using Planora.BLL.Handlers.Queries.Teachers; 
using Planora.BLL.Services;
using Planora.BLL.DTOs;
using Planora.BLL.DTOs.Queries;
using System.Linq;
using Moq;

public class IntegrationTests
{
    private PlanoraDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PlanoraDbContext>()
            .UseInMemoryDatabase(databaseName: dbName) 
            .Options;

        var context = new PlanoraDbContext(options);
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        return context;
    }

    private Planora.BLL.Handlers.Commands.Teachers.AddTeacherHandler GetAddTeacherHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.Teachers.AddTeacherHandler(c);
    private Planora.BLL.Handlers.Commands.Groups.AddGroupHandler GetAddGroupHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.Groups.AddGroupHandler(c);
    private Planora.BLL.Handlers.Commands.Students.AddStudentHandler GetAddStudentHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.Students.AddStudentHandler(c);
    private Planora.BLL.Handlers.Commands.Subjects.AddSubjectHandler GetAddSubjectHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.Subjects.AddSubjectHandler(c);
    private Planora.BLL.Handlers.Commands.Classrooms.AddClassroomHandler GetAddClassroomHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.Classrooms.AddClassroomHandler(c);
    private Planora.BLL.Handlers.Commands.TeachingAssignments.AddTeachingAssignmentHandler GetAddTeachingAssignmentHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.TeachingAssignments.AddTeachingAssignmentHandler(c);
    private Planora.BLL.Handlers.Commands.GroupDisciplineLists.AddGroupDisciplineListHandler GetAddGroupDisciplineListHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Commands.GroupDisciplineLists.AddGroupDisciplineListHandler(c);

    private Planora.BLL.Handlers.Commands.Workload.GenerateWorkloadHandler GetGenerateWorkloadHandler(PlanoraDbContext c)
    {
        var workloadService = new Planora.BLL.Services.WorkloadService(c);
        return new Planora.BLL.Handlers.Commands.Workload.GenerateWorkloadHandler(workloadService);
    }
    
    private Planora.BLL.Handlers.Queries.Classrooms.FindFreeClassroomHandler GetFindFreeClassroomQueryHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Queries.Classrooms.FindFreeClassroomHandler(c);
    
    private Planora.BLL.Handlers.Queries.Teachers.FindTeacherLocationHandler GetFindTeacherLocationQueryHandler(PlanoraDbContext c) => new Planora.BLL.Handlers.Queries.Teachers.FindTeacherLocationHandler(c);


    [Fact]
    public async Task Test01_AddTeacher_ValidData_ReturnsTrueAndSaves()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddTeacherHandler(context); 

        var command = new AddTeacherCommand(new AddTeacherDto { FullName = "Іван Петренко", Email = "ivan.petrenko@example.com", Faculty = "ФІОТ", Position = "Професор" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedUser = await context.Users.SingleOrDefaultAsync(u => u.Email == "ivan.petrenko@example.com");
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task Test04_AddClassroom_ValidData_ReturnsTrueAndSaves()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddClassroomHandler(context); 

        var command = new AddClassroomCommand(new AddClassroomDto { Number = "315", Building = "12", Capacity = 40, Faculty = "ФІОТ", HasComputers = true, HasProjector = true });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedClassroom = await context.Classrooms.SingleOrDefaultAsync(c => c.Number == "315" && c.Building == "12");
        Assert.NotNull(savedClassroom);
        Assert.True(savedClassroom.HasComputers);
    }

    [Fact]
    public async Task Test11_AddClassroom_ExistingNumberAndBuilding_ReturnsFalse()
    {
        string dbName = Guid.NewGuid().ToString(); 
        await using var context = CreateDbContext(dbName);
        context.Classrooms.Add(new Classroom { Number = "101", Building = "5", Capacity = 50, Faculty = "ФІОТ" });
        await context.SaveChangesAsync();

        var handler = GetAddClassroomHandler(context); 
        var command = new AddClassroomCommand(new AddClassroomDto { Number = "101", Building = "5", Capacity = 60, Faculty = "Інший" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        var count = await context.Classrooms.CountAsync(c => c.Number == "101" && c.Building == "5");
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Test12_AddStudent_NonExistingGroup_ReturnsFalse()
    {
        string dbName = Guid.NewGuid().ToString(); 
        await using var context = CreateDbContext(dbName);
        
        var studentHandler = GetAddStudentHandler(context); 
        var command = new AddStudentCommand(new AddStudentDto { FullName = "Тест Студент", Email = "test@ex.com", Faculty = "ФІОТ", GroupName = "Неіснуюча" });

        var result = await studentHandler.Handle(command, CancellationToken.None);

        Assert.False(result);
        var count = await context.Students.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Test17_FindFreeClassroom_OccupiedSlot_ReturnsEmptyList()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var classroom = new Classroom { Id = 1, Number = "101", Building = "1", Capacity = 30, HasProjector = true }; 
        var occupiedSchedule = new Schedule 
        { 
            ClassroomId = 1, DayOfWeek = 2, StartTime = new TimeOnly(10, 10), EndTime = new TimeOnly(11, 30), 
            SubjectId = 1, UserId = 1, GroupId = 1
        }; 
        context.Classrooms.Add(classroom);
        context.Schedules.Add(occupiedSchedule);
        await context.SaveChangesAsync();
        
        var handler = GetFindFreeClassroomQueryHandler(context); 
        var queryDto = new FindFreeClassroomQueryDto
        {
            DayOfWeek = 2, 
            StartTime = new TimeOnly(10, 10), 
            RequiredCapacity = 20,
            NeedsProjector = true
        };

        var results = await handler.Handle(new FindFreeClassroomQuery(queryDto), CancellationToken.None);

        Assert.Empty(results); 
    }

    [Fact]
    public async Task Test18_FindFreeClassroom_AvailableSlotWithRequirements_ReturnsCorrectClassroom()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var suitableClassroom = new Classroom { Id = 10, Number = "202", Building = "1", Capacity = 40, HasProjector = true, HasComputers = true }; 
        var unsuitableClassroom = new Classroom { Id = 20, Number = "303", Building = "1", Capacity = 50, HasProjector = false, HasComputers = false };
        context.Classrooms.AddRange(suitableClassroom, unsuitableClassroom);
        await context.SaveChangesAsync();
        
        var handler = GetFindFreeClassroomQueryHandler(context); 
        var queryDto = new FindFreeClassroomQueryDto
        {
            DayOfWeek = 4, 
            StartTime = new TimeOnly(13, 30), 
            RequiredCapacity = 35, 
            NeedsProjector = true, 
            Building = "1"
        };

        var results = await handler.Handle(new FindFreeClassroomQuery(queryDto), CancellationToken.None);

        Assert.Single(results); 
        Assert.Equal("202", results.First().Number); 
    }

    [Fact]
    public async Task Test23_AddSubject_ValidData_ReturnsTrueAndSaves()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddSubjectHandler(context); 

        var command = new AddSubjectCommand(new AddSubjectDto { Name = "Теория ймовірності", Type = "Лекція", Duration = 60 });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedSubject = await context.Subjects.SingleOrDefaultAsync(s => s.Name == "Теория ймовірності");
        Assert.NotNull(savedSubject);
    }


    [Fact]
    public async Task Test25_AddTeachingAssignment_ValidData_ReturnsTrueAndSaves()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var teacher = new User { FullName = "Викладач 25", Email = "test25@ex.com" }; 
        var subject = new Subject { Name = "Алгоритми", Type = "Практика", Duration = 40 };
        context.Users.Add(teacher);
        context.Subjects.Add(subject);
        await context.SaveChangesAsync();
        
        var assignmentHandler = GetAddTeachingAssignmentHandler(context); 
        var command = new AddTeachingAssignmentCommand(new AddTeachingAssignmentDto { UserId = teacher.Id, SubjectId = subject.Id, Hours = 40 });

        var result = await assignmentHandler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedAssignment = await context.TeachingAssignments.SingleOrDefaultAsync(ta => ta.UserId == teacher.Id);
        Assert.NotNull(savedAssignment);
        Assert.Equal(40, savedAssignment.Hours);
    }
    
    [Fact]
    public async Task Test26_AddGroupDisciplineList_ValidData_SavesWithDependencies()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var group = new Group { Name = "КН-24", Faculty = "ФІОТ", StudentCount = 30, Number = "KN-24" };
        var subject = new Subject { Name = "Web", Type = "Лекція", Duration = 30 };
        context.Groups.Add(group);
        context.Subjects.Add(subject);
        await context.SaveChangesAsync();
        
        var disciplineHandler = GetAddGroupDisciplineListHandler(context); 
        var command = new AddGroupDisciplineListCommand(new AddGroupDisciplineListDto { GroupId = group.Id, SubjectId = subject.Id, Hours = 30 });

        var result = await disciplineHandler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedDiscipline = await context.GroupDisciplineLists.SingleOrDefaultAsync();
        Assert.NotNull(savedDiscipline);
        Assert.Equal(30, savedDiscipline.Hours);
    }
    
    [Fact]
    public async Task Test33_AddClassroom_ValidData_HasNoRequirements()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddClassroomHandler(context); 

        var command = new AddClassroomCommand(new AddClassroomDto { Number = "111", Building = "3", Capacity = 20, Faculty = "ФІОТ", HasComputers = false, HasProjector = false });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedClassroom = await context.Classrooms.SingleOrDefaultAsync(c => c.Number == "111");
        Assert.NotNull(savedClassroom);
        Assert.False(savedClassroom.HasComputers);
    }
    
    [Fact]
    public async Task Test34_AddSubject_ValidData_AllowsEmptyRequirements()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddSubjectHandler(context); 

        var command = new AddSubjectCommand(new AddSubjectDto { Name = "Дизайн", Type = "Практика", Duration = 30, Requirements = "" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedSubject = await context.Subjects.SingleOrDefaultAsync(s => s.Name == "Дизайн");
        Assert.NotNull(savedSubject);
        Assert.Equal("", savedSubject.Requirements);
    }
    
    [Fact]
    public async Task Test35_AddTeacher_ReturnsTrueIfPositionIsEmpty()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddTeacherHandler(context); 

        var command = new AddTeacherCommand(new AddTeacherDto { FullName = "Новий Викладач", Email = "new@example.com", Faculty = "ФІОТ", Position = "" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedUser = await context.Users.SingleOrDefaultAsync(u => u.Email == "new@example.com");
        Assert.NotNull(savedUser);
    }


    [Fact]
    public async Task Test40_FindFreeClassroom_FiltersByCapacity()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var c_small = new Classroom { Id = 1, Number = "10", Capacity = 20 }; 
        var c_large = new Classroom { Id = 2, Number = "20", Capacity = 50 };

        context.Classrooms.AddRange(c_small, c_large);
        await context.SaveChangesAsync();
        
        var handler = GetFindFreeClassroomQueryHandler(context); 
        var queryDto = new FindFreeClassroomQueryDto
        {
            DayOfWeek = 1, 
            StartTime = new TimeOnly(10, 10), 
            RequiredCapacity = 40, 
        };

        var results = await handler.Handle(new FindFreeClassroomQuery(queryDto), CancellationToken.None);

        Assert.Single(results); 
        Assert.Equal("20", results.First().Number);
    }


    [Fact]
    public async Task Test43_FindFreeClassroom_FiltersByBuilding()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var c_b1 = new Classroom { Id = 1, Number = "100", Building = "1", Capacity = 30 }; 
        var c_b2 = new Classroom { Id = 2, Number = "200", Building = "2", Capacity = 30 };

        context.Classrooms.AddRange(c_b1, c_b2);
        await context.SaveChangesAsync();
        
        var handler = GetFindFreeClassroomQueryHandler(context); 
        var queryDto = new FindFreeClassroomQueryDto
        {
            DayOfWeek = 1, 
            StartTime = new TimeOnly(10, 10), 
            RequiredCapacity = 10,
            Building = "2" 
        };

        var results = await handler.Handle(new FindFreeClassroomQuery(queryDto), CancellationToken.None);

        Assert.Single(results); 
        Assert.Equal("200", results.First().Number);
    }
    
    [Fact]
    public async Task Test44_AddClassroom_ValidData_NoRequirementsSpecified()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        var handler = GetAddClassroomHandler(context); 

        var command = new AddClassroomCommand(new AddClassroomDto { Number = "707", Building = "7", Capacity = 60, Faculty = "ФІОТ" }); // No bools specified

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        var savedClassroom = await context.Classrooms.SingleOrDefaultAsync(c => c.Number == "707");
        Assert.NotNull(savedClassroom);
        Assert.False(savedClassroom.HasComputers);
        Assert.False(savedClassroom.HasProjector);
    }
    
    [Fact]
    public async Task Test45_AddTeachingAssignment_ExistingAssignment_ReturnsFalse()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);
        
        var teacher = new User { Id = 100, FullName = "T100", Email = "t100@ex.com" }; 
        var subject = new Subject { Id = 100, Name = "S100", Type = "Лекція", Duration = 10 };
        context.Users.Add(teacher);
        context.Subjects.Add(subject);
        context.TeachingAssignments.Add(new TeachingAssignment { UserId = 100, SubjectId = 100, Hours = 10 });
        await context.SaveChangesAsync();
        
        var assignmentHandler = GetAddTeachingAssignmentHandler(context); 
        var command = new AddTeachingAssignmentCommand(new AddTeachingAssignmentDto { UserId = 100, SubjectId = 100, Hours = 20 });

        var result = await assignmentHandler.Handle(command, CancellationToken.None);

        Assert.False(result);
        var count = await context.TeachingAssignments.CountAsync();
        Assert.Equal(1, count);
    }
    
    [Fact]
    public async Task Test46_AddGroupDisciplineList_ExistingAssignment_ReturnsFalse()
    {
        string dbName = Guid.NewGuid().ToString();
        await using var context = CreateDbContext(dbName);

        var group = new Group { Id = 100, Name = "Група 100", Faculty = "ФІОТ", StudentCount = 30, Number = "G-100" };
        var subject = new Subject { Id = 100, Name = "Дисципліна 100", Type = "Лекція", Duration = 60 };
        context.Groups.Add(group);
        context.Subjects.Add(subject);

        context.GroupDisciplineLists.Add(new GroupDisciplineList { GroupId = 100, SubjectId = 100, Hours = 60 });
        await context.SaveChangesAsync();
        
        var disciplineHandler = GetAddGroupDisciplineListHandler(context); 
        var command = new AddGroupDisciplineListCommand(new AddGroupDisciplineListDto { GroupId = 100, SubjectId = 100, Hours = 60 });

        var result = await disciplineHandler.Handle(command, CancellationToken.None);

        Assert.False(result);
        var count = await context.GroupDisciplineLists.CountAsync();
        Assert.Equal(1, count);
    }
    
    [Fact]
    public async Task Test47_AddStudent_ExistingEmail_ReturnsFalse()
    {
        string dbName = Guid.NewGuid().ToString(); 
        await using var context = CreateDbContext(dbName);

        var group = new Group { Name = "КН-25", Faculty = "ФІОТ", StudentCount = 30, Number = "KN-25" };
        context.Groups.Add(group);
        context.Students.Add(new Student { FullName = "Іван Іванов", Email = "s_exists@example.com", GroupId = group.Id }); 
        await context.SaveChangesAsync();

        var studentHandler = GetAddStudentHandler(context); 
        var command = new AddStudentCommand(new AddStudentDto { FullName = "Петро Петров", Email = "s_exists@example.com", Faculty = "ФІОТ", GroupName = "КН-25" });

        var result = await studentHandler.Handle(command, CancellationToken.None);

        Assert.False(result);
        var usersCount = await context.Students.CountAsync(u => u.Email == "s_exists@example.com");
        Assert.Equal(1, usersCount);
    }

}