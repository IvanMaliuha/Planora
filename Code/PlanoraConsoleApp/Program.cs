using System;
using Npgsql;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var connectionString = "Host=localhost;Port=5432;Database=planora_db;Username=postgres;Password=maksimdata1234";
        
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        InsertTestData(connection);
        DisplayAllData(connection);
    }
    
    static void InsertTestData(NpgsqlConnection connection)
    {
        InsertUsers(connection);
        InsertAdministrators(connection);
        InsertGroups(connection);
        InsertTeachers(connection);
        InsertStudents(connection);
        InsertSubjects(connection);
        InsertAudience(connection);
        InsertSchedule(connection);
        InsertRoomSearch(connection);
        InsertTeachingAssignment(connection);
        InsertGroupDisciplineList(connection);
        InsertWorkload(connection);
    }
    
    static void InsertUsers(NpgsqlConnection connection)
    {
        var users = new List<(string, string, string, string, string)>();
        for (int i = 1; i <= 35; i++)
        {
            string role = i switch
            {
                1 => "admin",
                <= 10 => "teacher",
                _ => "student"
            };
            
            users.Add((
                $"User {i}",
                $"user{i}@university.com",
                $"user{i}",
                $"password{i}",
                role
            ));
        }
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        foreach (var user in users)
        {
            cmd.CommandText = @"
                INSERT INTO Users (full_name, email, login, password_hash, role) 
                VALUES (@name, @email, @login, @password, @role)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("name", user.Item1);
            cmd.Parameters.AddWithValue("email", user.Item2);
            cmd.Parameters.AddWithValue("login", user.Item3);
            cmd.Parameters.AddWithValue("password", user.Item4);
            cmd.Parameters.AddWithValue("role", user.Item5);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertAdministrators(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        cmd.CommandText = "INSERT INTO Administrator (user_id) VALUES (1)";
        cmd.ExecuteNonQuery();
    }
    
    static void InsertGroups(NpgsqlConnection connection)
    {
        var groups = new List<(string, int, string)>();
        for (int i = 1; i <= 35; i++)
        {
            groups.Add((
                $"Group {((i-1) % 7) + 1}-{(char)('A' + (i-1) / 7)}",
                25 + (i % 10),
                $"Faculty {((i-1) % 5) + 1}"
            ));
        }
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        foreach (var group in groups)
        {
            cmd.CommandText = @"
                INSERT INTO Groups (name, amount_students, faculty) 
                VALUES (@name, @amount, @faculty)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("name", group.Item1);
            cmd.Parameters.AddWithValue("amount", group.Item2);
            cmd.Parameters.AddWithValue("faculty", group.Item3);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertTeachers(NpgsqlConnection connection)
    {
        var departments = new[] { "Computer Science", "Mathematics", "Physics", "Engineering", "Chemistry" };
        var positions = new[] { "Professor", "Associate Professor", "Assistant Professor", "Lecturer" };
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 2; i <= 10; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Teachers (user_id, department, position, email) 
                VALUES (@user_id, @department, @position, @email)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", i);
            cmd.Parameters.AddWithValue("department", departments[(i-2) % departments.Length]);
            cmd.Parameters.AddWithValue("position", positions[(i-2) % positions.Length]);
            cmd.Parameters.AddWithValue("email", $"teacher{i}@university.com");
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertStudents(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 11; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Students (user_id, group_id, faculty) 
                VALUES (@user_id, @group_id, @faculty)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", i);
            cmd.Parameters.AddWithValue("group_id", (i - 10));
            cmd.Parameters.AddWithValue("faculty", $"Faculty {((i-11) % 5) + 1}");
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertSubjects(NpgsqlConnection connection)
    {
        var subjects = new List<(string, int, int)>();
        var subjectNames = new[] 
        {
            "Mathematics", "Physics", "Programming", "Algorithms", "Database Systems",
            "Web Development", "Operating Systems", "Computer Networks", "Software Engineering",
            "Data Structures", "Artificial Intelligence", "Machine Learning", "Cyber Security"
        };
        
        for (int i = 0; i < 35; i++)
        {
            subjects.Add((
                $"{subjectNames[i % subjectNames.Length]} {(i / subjectNames.Length) + 1}",
                3 + (i % 4),
                1 + (i % 2)
            ));
        }
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        foreach (var subject in subjects)
        {
            cmd.CommandText = @"
                INSERT INTO Subjects (name, credits, semester) 
                VALUES (@name, @credits, @semester)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("name", subject.Item1);
            cmd.Parameters.AddWithValue("credits", subject.Item2);
            cmd.Parameters.AddWithValue("semester", subject.Item3);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertAudience(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Audience (number, corps, capacity, has_projector, has_computers) 
                VALUES (@number, @corps, @capacity, @projector, @computers)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("number", $"{(i % 10) + 1}{(char)('A' + (i / 10))}");
            cmd.Parameters.AddWithValue("corps", $"Corps {(i % 5) + 1}");
            cmd.Parameters.AddWithValue("capacity", 20 + (i * 2));
            cmd.Parameters.AddWithValue("projector", i % 3 == 0);
            cmd.Parameters.AddWithValue("computers", i % 2 == 0);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertSchedule(NpgsqlConnection connection)
    {
        var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Schedule (subject_id, teacher_id, group_id, audience_id, day, start_time, end_time, lesson_type) 
                VALUES (@subject_id, @teacher_id, @group_id, @audience_id, @day, @start_time, @end_time, @type)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("subject_id", i);
            cmd.Parameters.AddWithValue("teacher_id", 2 + ((i-1) % 9));
            cmd.Parameters.AddWithValue("group_id", i);
            cmd.Parameters.AddWithValue("audience_id", i);
            cmd.Parameters.AddWithValue("day", days[i % days.Length]);
            cmd.Parameters.AddWithValue("start_time", TimeSpan.FromHours(8 + (i % 6)));
            cmd.Parameters.AddWithValue("end_time", TimeSpan.FromHours(10 + (i % 6)));
            cmd.Parameters.AddWithValue("type", i % 3 == 0 ? "Lecture" : "Practice");
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertRoomSearch(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO RoomSearch (user_id, corps, type, capacity, has_computers, has_projector) 
                VALUES (@user_id, @corps, @type, @capacity, @computers, @projector)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", 2 + ((i-1) % 34));
            cmd.Parameters.AddWithValue("corps", $"Corps {(i % 5) + 1}");
            cmd.Parameters.AddWithValue("type", i % 2 == 0 ? "Lecture" : "Laboratory");
            cmd.Parameters.AddWithValue("capacity", 25 + (i * 3));
            cmd.Parameters.AddWithValue("computers", i % 3 == 0);
            cmd.Parameters.AddWithValue("projector", i % 2 == 0);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertTeachingAssignment(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO TeachingAssignment (teacher_id, subject_id, group_id, hours, requirements) 
                VALUES (@teacher_id, @subject_id, @group_id, @hours, @requirements)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("teacher_id", 2 + ((i-1) % 9));
            cmd.Parameters.AddWithValue("subject_id", i);
            cmd.Parameters.AddWithValue("group_id", i);
            cmd.Parameters.AddWithValue("hours", 30 + (i % 20));
            cmd.Parameters.AddWithValue("requirements", $"Requirements for assignment {i}");
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertGroupDisciplineList(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO GroupDisciplineList (group_id, subject_id) 
                VALUES (@group_id, @subject_id)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("group_id", i);
            cmd.Parameters.AddWithValue("subject_id", i);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertWorkload(NpgsqlConnection connection)
    {
        var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 35; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Workload (group_id, teacher_id, subject_id, audience_id, day, start_time, end_time, hours_needed) 
                VALUES (@group_id, @teacher_id, @subject_id, @audience_id, @day, @start_time, @end_time, @hours)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("group_id", i);
            cmd.Parameters.AddWithValue("teacher_id", 2 + ((i-1) % 9));
            cmd.Parameters.AddWithValue("subject_id", i);
            cmd.Parameters.AddWithValue("audience_id", i);
            cmd.Parameters.AddWithValue("day", days[i % days.Length]);
            cmd.Parameters.AddWithValue("start_time", TimeSpan.FromHours(9 + (i % 5)));
            cmd.Parameters.AddWithValue("end_time", TimeSpan.FromHours(11 + (i % 5)));
            cmd.Parameters.AddWithValue("hours", 120 + (i * 2));
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void DisplayAllData(NpgsqlConnection connection)
    {
        var tables = new[] 
        {
            "Users", "Administrator", "Teachers", "Groups", "Students",
            "Subjects", "Audience", "Schedule", "RoomSearch", "TeachingAssignment",
            "GroupDisciplineList", "Workload"
        };
        
        foreach (var table in tables)
        {
            Console.WriteLine($"\n=== {table} ===");
            
            using var cmd = new NpgsqlCommand($"SELECT * FROM {table} LIMIT 35", connection);
            using var reader = cmd.ExecuteReader();
            
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.Write($"{reader.GetName(i),-20}");
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', reader.FieldCount * 20));
            
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader[i] is DBNull ? "NULL" : reader[i].ToString();
                    Console.Write($"{value,-20}");
                }
                Console.WriteLine();
            }
            
            reader.Close();
        }
    }
}