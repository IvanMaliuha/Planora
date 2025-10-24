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
        InsertGroups(connection);
        InsertTeachers(connection);
        InsertAdministrator(connection);
        InsertStudents(connection);
        InsertSubjects(connection);
        InsertClassrooms(connection);
        InsertTeachingAssignment(connection);
        InsertGroupDisciplineList(connection);
        InsertWorkload(connection);
        InsertSchedule(connection);
    }
    
    static void InsertUsers(NpgsqlConnection connection)
    {
        var users = new List<(string, string, string, string)>();
        for (int i = 1; i <= 50; i++)
        {
            string role = i switch
            {
                1 => "admin",
                <= 15 => "teacher",
                _ => "student"
            };
            
            users.Add((
                $"User {i}",
                $"user{i}@university.com",
                $"password_hash_{i}",
                role
            ));
        }
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        foreach (var user in users)
        {
            cmd.CommandText = @"
                INSERT INTO Users (full_name, email, password_hash, role) 
                VALUES (@name, @email, @password, @role)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("name", user.Item1);
            cmd.Parameters.AddWithValue("email", user.Item2);
            cmd.Parameters.AddWithValue("password", user.Item3);
            cmd.Parameters.AddWithValue("role", user.Item4);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertAdministrator(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        cmd.CommandText = "INSERT INTO Administrator (user_id) VALUES (1)";
        cmd.ExecuteNonQuery();
    }
    
    static void InsertGroups(NpgsqlConnection connection)
    {
        var groups = new List<(string, string, int)>();
        for (int i = 1; i <= 30; i++)
        {
            groups.Add((
                $"Group {((i-1) % 10) + 1}-{(char)('A' + (i-1) / 10)}",
                $"Faculty {((i-1) % 5) + 1}",
                20 + (i * 2)
            ));
        }
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        foreach (var group in groups)
        {
            cmd.CommandText = @"
                INSERT INTO Groups (name, faculty, student_count) 
                VALUES (@name, @faculty, @student_count)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("name", group.Item1);
            cmd.Parameters.AddWithValue("faculty", group.Item2);
            cmd.Parameters.AddWithValue("student_count", group.Item3);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertTeachers(NpgsqlConnection connection)
    {
        var faculties = new[] { "Computer Science", "Mathematics", "Physics", "Engineering", "Chemistry", "Biology", "Economics" };
        var positions = new[] { "Professor", "Associate Professor", "Assistant Professor", "Lecturer", "Senior Lecturer" };
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 2; i <= 15; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Teachers (user_id, faculty, position) 
                VALUES (@user_id, @faculty, @position)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", i);
            cmd.Parameters.AddWithValue("faculty", faculties[(i-2) % faculties.Length]);
            cmd.Parameters.AddWithValue("position", positions[(i-2) % positions.Length]);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertStudents(NpgsqlConnection connection)
    {
        var faculties = new[] { "Computer Science", "Mathematics", "Physics", "Engineering", "Chemistry", "Biology", "Economics" };
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 16; i <= 50; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Students (user_id, group_id, faculty) 
                VALUES (@user_id, @group_id, @faculty)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", i);
            cmd.Parameters.AddWithValue("group_id", ((i - 16) % 30) + 1);
            cmd.Parameters.AddWithValue("faculty", faculties[(i-16) % faculties.Length]);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertSubjects(NpgsqlConnection connection)
    {
        var subjects = new List<(string, string, string, int)>();
        var subjectNames = new[] 
        {
            "Mathematics", "Physics", "Programming", "Algorithms", "Database Systems",
            "Web Development", "Operating Systems", "Computer Networks", "Software Engineering",
            "Data Structures", "Artificial Intelligence", "Machine Learning", "Cyber Security",
            "Data Science", "Computer Graphics", "Mobile Development", "Cloud Computing",
            "Big Data", "Internet of Things", "Blockchain Technology"
        };
        
        var subjectTypes = new[] { "Lecture", "Practice", "Laboratory" };
        
        for (int i = 0; i < 30; i++)
        {
            subjects.Add((
                $"{subjectNames[i % subjectNames.Length]} {(i / subjectNames.Length) + 1}",
                subjectTypes[i % subjectTypes.Length],
                $"Requirements for {subjectNames[i % subjectNames.Length]} {(i / subjectNames.Length) + 1}",
                30 + (i * 3)
            ));
        }
        
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        foreach (var subject in subjects)
        {
            cmd.CommandText = @"
                INSERT INTO Subjects (name, type, requirements, duration) 
                VALUES (@name, @type, @requirements, @duration)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("name", subject.Item1);
            cmd.Parameters.AddWithValue("type", subject.Item2);
            cmd.Parameters.AddWithValue("requirements", subject.Item3);
            cmd.Parameters.AddWithValue("duration", subject.Item4);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertClassrooms(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 30; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Classrooms (number, building, capacity, faculty, has_computers, has_projector) 
                VALUES (@number, @building, @capacity, @faculty, @computers, @projector)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("number", $"{100 + i}");
            cmd.Parameters.AddWithValue("building", $"Building {(i % 6) + 1}");
            cmd.Parameters.AddWithValue("capacity", 20 + (i * 4));
            cmd.Parameters.AddWithValue("faculty", $"Faculty {(i % 5) + 1}");
            cmd.Parameters.AddWithValue("computers", i % 2 == 0);
            cmd.Parameters.AddWithValue("projector", i % 3 == 0 || i % 5 == 0);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertSchedule(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        var weekTypes = new[] { "both", "num", "denom" };
        
        for (int i = 1; i <= 30; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Schedule (user_id, subject_id, group_id, classroom_id, day_of_week, start_time, end_time, week_type) 
                VALUES (@user_id, @subject_id, @group_id, @classroom_id, @day_of_week, @start_time, @end_time, @week_type)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", 2 + ((i-1) % 14));
            cmd.Parameters.AddWithValue("subject_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("group_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("classroom_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("day_of_week", (i % 5) + 1);
            cmd.Parameters.AddWithValue("start_time", TimeSpan.FromHours(8 + ((i-1) % 8)));
            cmd.Parameters.AddWithValue("end_time", TimeSpan.FromHours(10 + ((i-1) % 8)));
            cmd.Parameters.AddWithValue("week_type", weekTypes[i % weekTypes.Length]);
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertTeachingAssignment(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 30; i++)
        {
            cmd.CommandText = @"
                INSERT INTO TeachingAssignment (user_id, subject_id, hours) 
                VALUES (@user_id, @subject_id, @hours)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", 2 + ((i-1) % 14));
            cmd.Parameters.AddWithValue("subject_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("hours", 25 + (i * 2));
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertGroupDisciplineList(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 30; i++)
        {
            cmd.CommandText = @"
                INSERT INTO GroupDisciplineList (group_id, subject_id, hours) 
                VALUES (@group_id, @subject_id, @hours)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("group_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("subject_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("hours", 40 + (i * 3));
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void InsertWorkload(NpgsqlConnection connection)
    {
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        
        for (int i = 1; i <= 30; i++)
        {
            cmd.CommandText = @"
                INSERT INTO Workload (user_id, subject_id, group_id, duration) 
                VALUES (@user_id, @subject_id, @group_id, @duration)";
            
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("user_id", 2 + ((i-1) % 14));
            cmd.Parameters.AddWithValue("subject_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("group_id", ((i-1) % 30) + 1);
            cmd.Parameters.AddWithValue("duration", 50 + (i * 4));
            
            cmd.ExecuteNonQuery();
        }
    }
    
    static void DisplayAllData(NpgsqlConnection connection)
    {
        var tables = new[] 
        {
            "Users", "Administrator", "Teachers", "Groups", "Students",
            "Subjects", "Classrooms", "Schedule", "TeachingAssignment",
            "GroupDisciplineList", "Workload"
        };
        
        foreach (var table in tables)
        {
            Console.WriteLine($"\n=== {table} ===");
            
            using var cmd = new NpgsqlCommand($"SELECT COUNT(*) FROM {table}", connection);
            var count = cmd.ExecuteScalar();
            Console.WriteLine($"Total records: {count}");
            
            using var cmdSelect = new NpgsqlCommand($"SELECT * FROM {table} LIMIT 10", connection);
            using var reader = cmdSelect.ExecuteReader();
            
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