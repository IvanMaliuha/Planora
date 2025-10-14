using System;
using Npgsql;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Host=localhost;Port=5432;Database=planora_db;Username=postgres;Password=maksimdata1234";

        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();

        var random = new Random();
        NpgsqlCommand cmd;

        //Teachers
        for (int i = 1; i <= 5; i++)
        {
            string name = $"Teacher {i}";
            string email = $"teacher{i}@planora.com";
            string login = $"teacher{i}";
            string pass = $"tpass{i}";
            string department = "Computer Science";
            string position = (i % 2 == 0) ? "Assistant" : "Professor";

            cmd = new NpgsqlCommand(
                "INSERT INTO Users (full_name, email, login, password_hash, role) " +
                "VALUES (@n, @e, @l, @p, 'teacher') RETURNING user_id;", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@l", login);
            cmd.Parameters.AddWithValue("@p", pass);
            int teacherId = Convert.ToInt32(cmd.ExecuteScalar());

            cmd = new NpgsqlCommand(
                "INSERT INTO Teachers (user_id, department, position, email) VALUES (@id, @d, @pos, @em);", conn);
            cmd.Parameters.AddWithValue("@id", teacherId);
            cmd.Parameters.AddWithValue("@d", department);
            cmd.Parameters.AddWithValue("@pos", position);
            cmd.Parameters.AddWithValue("@em", email);
            cmd.ExecuteNonQuery();
        }

        //Groups
        for (int i = 1; i <= 4; i++)
        {
            string name = $"CS-{i}01";
            string faculty = "Faculty of Informatics";
            int amount = random.Next(10, 30);

            cmd = new NpgsqlCommand(
                "INSERT INTO Groups (name, amount_students, faculty) VALUES (@n, @a, @f);", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@a", amount);
            cmd.Parameters.AddWithValue("@f", faculty);
            cmd.ExecuteNonQuery();
        }

        //Students
        for (int i = 1; i <= 20; i++)
        {
            string name = $"Student {i}";
            string email = $"student{i}@planora.com";
            string login = $"student{i}";
            string pass = $"spass{i}";
            int groupId = random.Next(1, 5);
            string faculty = "Faculty of Informatics";

            cmd = new NpgsqlCommand(
                "INSERT INTO Users (full_name, email, login, password_hash, role) " +
                "VALUES (@n, @e, @l, @p, 'student') RETURNING user_id;", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@l", login);
            cmd.Parameters.AddWithValue("@p", pass);
            int studentId = Convert.ToInt32(cmd.ExecuteScalar());

            cmd = new NpgsqlCommand(
                "INSERT INTO Students (user_id, group_id, faculty) VALUES (@u, @g, @f);", conn);
            cmd.Parameters.AddWithValue("@u", studentId);
            cmd.Parameters.AddWithValue("@g", groupId);
            cmd.Parameters.AddWithValue("@f", faculty);
            cmd.ExecuteNonQuery();
        }

        //Subjects
        for (int i = 1; i <= 5; i++)
        {
            string name = $"Subject {i}";
            int credits = random.Next(2, 6);
            int semester = random.Next(1, 8);

            cmd = new NpgsqlCommand(
                "INSERT INTO Subjects (name, credits, semester) VALUES (@n, @c, @s);", conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@c", credits);
            cmd.Parameters.AddWithValue("@s", semester);
            cmd.ExecuteNonQuery();
        }

        //Audience
        for (int i = 1; i <= 5; i++)
        {
            string number = $"{100 + i}";
            string corps = $"Building {random.Next(1, 3)}";
            int capacity = random.Next(20, 80);
            bool projector = random.Next(0, 2) == 1;
            bool computers = random.Next(0, 2) == 1;

            cmd = new NpgsqlCommand(
                "INSERT INTO Audience (number, corps, capacity, has_projector, has_computers) " +
                "VALUES (@n, @c, @cap, @p, @comp);", conn);
            cmd.Parameters.AddWithValue("@n", number);
            cmd.Parameters.AddWithValue("@c", corps);
            cmd.Parameters.AddWithValue("@cap", capacity);
            cmd.Parameters.AddWithValue("@p", projector);
            cmd.Parameters.AddWithValue("@comp", computers);
            cmd.ExecuteNonQuery();
        }

        //TeachingAssignment
        for (int i = 1; i <= 10; i++)
        {
            int teacherId = random.Next(1, 6);
            int subjectId = random.Next(1, 6);
            int groupId = random.Next(1, 5);
            int hours = random.Next(10, 40);
            string requirements = "Standard classroom requirements";

            cmd = new NpgsqlCommand(
                "INSERT INTO TeachingAssignment (teacher_id, subject_id, group_id, hours, requirements) " +
                "VALUES (@t, @s, @g, @h, @r);", conn);
            cmd.Parameters.AddWithValue("@t", teacherId);
            cmd.Parameters.AddWithValue("@s", subjectId);
            cmd.Parameters.AddWithValue("@g", groupId);
            cmd.Parameters.AddWithValue("@h", hours);
            cmd.Parameters.AddWithValue("@r", requirements);
            cmd.ExecuteNonQuery();
        }

        //Workload
        for (int i = 1; i <= 10; i++)
        {
            int groupId = random.Next(1, 5);
            int teacherId = random.Next(1, 6);
            int subjectId = random.Next(1, 6);
            int audienceId = random.Next(1, 6);
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            string day = days[random.Next(days.Length)];
            TimeSpan start = new TimeSpan(random.Next(8, 16), 0, 0);
            TimeSpan end = start.Add(new TimeSpan(2, 0, 0));

            cmd = new NpgsqlCommand(
                "INSERT INTO Workload (group_id, teacher_id, subject_id, audience_id, day, start_time, end_time, hours_needed) " +
                "VALUES (@g, @t, @s, @a, @d, @st, @en, 2);", conn);
            cmd.Parameters.AddWithValue("@g", groupId);
            cmd.Parameters.AddWithValue("@t", teacherId);
            cmd.Parameters.AddWithValue("@s", subjectId);
            cmd.Parameters.AddWithValue("@a", audienceId);
            cmd.Parameters.AddWithValue("@d", day);
            cmd.Parameters.AddWithValue("@st", start);
            cmd.Parameters.AddWithValue("@en", end);
            cmd.ExecuteNonQuery();
        }

        //Schedule
        for (int i = 1; i <= 5; i++)
        {
            int subjectId = random.Next(1, 6);
            int teacherId = random.Next(1, 6);
            int groupId = random.Next(1, 5);
            int audienceId = random.Next(1, 6);
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            string day = days[random.Next(days.Length)];
            TimeSpan start = new TimeSpan(random.Next(8, 16), 0, 0);
            TimeSpan end = start.Add(new TimeSpan(1, 30, 0));
            string type = "Lecture";

            cmd = new NpgsqlCommand(
                "INSERT INTO Schedule (subject_id, teacher_id, group_id, audience_id, day, start_time, end_time, lesson_type) " +
                "VALUES (@s, @t, @g, @a, @d, @st, @en, @lt);", conn);
            cmd.Parameters.AddWithValue("@s", subjectId);
            cmd.Parameters.AddWithValue("@t", teacherId);
            cmd.Parameters.AddWithValue("@g", groupId);
            cmd.Parameters.AddWithValue("@a", audienceId);
            cmd.Parameters.AddWithValue("@d", day);
            cmd.Parameters.AddWithValue("@st", start);
            cmd.Parameters.AddWithValue("@en", end);
            cmd.Parameters.AddWithValue("@lt", type);
            cmd.ExecuteNonQuery();
        }

        //RoomSearch
        for (int i = 1; i <= 3; i++)
        {
            int userId = random.Next(6, 26);
            string corps = $"Building {random.Next(1, 3)}";
            string type = "Lecture room";
            int capacity = random.Next(20, 80);
            bool hasComputers = random.Next(0, 2) == 1;
            bool hasProjector = random.Next(0, 2) == 1;

            cmd = new NpgsqlCommand(
                "INSERT INTO RoomSearch (user_id, corps, type, capacity, has_computers, has_projector) " +
                "VALUES (@u, @c, @t, @cap, @hc, @hp);", conn);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@c", corps);
            cmd.Parameters.AddWithValue("@t", type);
            cmd.Parameters.AddWithValue("@cap", capacity);
            cmd.Parameters.AddWithValue("@hc", hasComputers);
            cmd.Parameters.AddWithValue("@hp", hasProjector);
            cmd.ExecuteNonQuery();
        }

        Console.WriteLine("Тестові дані успішно згенеровані.");
    }
}
