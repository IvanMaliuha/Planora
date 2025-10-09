-- ===========================================
--   CREATE DATABASE STRUCTURE FOR DESKTOP APP
-- ===========================================

-- ===========================================
--   TABLE: Roles (опціонально, якщо role у Users зберігається як текст)
-- ===========================================
CREATE TABLE Roles (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO Roles (role_name) VALUES
('admin'),
('teacher'),
('student');

-- ===========================================
--   TABLE: Users
-- ===========================================
CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role_id INT REFERENCES Roles(role_id) ON DELETE SET NULL
);

-- ===========================================
--   TABLE: Teachers
-- ===========================================
CREATE TABLE Teachers (
    teacher_id SERIAL PRIMARY KEY,
    user_id INT UNIQUE REFERENCES Users(user_id) ON DELETE CASCADE,
    department VARCHAR(100),
    position VARCHAR(100),
    email VARCHAR(100)
);

-- ===========================================
--   TABLE: Groups
-- ===========================================
CREATE TABLE Groups (
    group_id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    amount_students INT,
    faculty VARCHAR(100)
);

-- ===========================================
--   TABLE: Students
-- ===========================================
CREATE TABLE Students (
    student_id SERIAL PRIMARY KEY,
    user_id INT UNIQUE REFERENCES Users(user_id) ON DELETE CASCADE,
    group_id INT REFERENCES Groups(group_id) ON DELETE SET NULL,
    faculty VARCHAR(100)
);

-- ===========================================
--   TABLE: Subjects
-- ===========================================
CREATE TABLE Subjects (
    subject_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    credits INT,
    semester INT
);

-- ===========================================
--   TABLE: Audience
-- ===========================================
CREATE TABLE Audience (
    audience_id SERIAL PRIMARY KEY,
    number VARCHAR(10) NOT NULL,
    corps VARCHAR(10),
    capacity INT,
    has_projector BOOLEAN DEFAULT FALSE,
    has_computers BOOLEAN DEFAULT FALSE
);

-- ===========================================
--   TABLE: Schedules
-- ===========================================
CREATE TABLE Schedules (
    schedule_id SERIAL PRIMARY KEY,
    subject_id INT REFERENCES Subjects(subject_id) ON DELETE CASCADE,
    teacher_id INT REFERENCES Teachers(teacher_id) ON DELETE CASCADE,
    group_id INT REFERENCES Groups(group_id) ON DELETE CASCADE,
    audience_id INT REFERENCES Audience(audience_id) ON DELETE SET NULL,
    day VARCHAR(20) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    lesson_type VARCHAR(50)
);

-- ===========================================
--   TABLE: RoomSearches
-- ===========================================
CREATE TABLE RoomSearches (
    search_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES Users(user_id) ON DELETE CASCADE,
    corps VARCHAR(10),
    type VARCHAR(50),
    capacity INT,
    has_computers BOOLEAN DEFAULT FALSE,
    has_projector BOOLEAN DEFAULT FALSE
);

-- ===========================================
--   TABLE: Tasks (для студентів від викладачів)
-- ===========================================
CREATE TABLE Tasks (
    task_id SERIAL PRIMARY KEY,
    teacher_id INT REFERENCES Teachers(teacher_id) ON DELETE CASCADE,
    subject_id INT REFERENCES Subjects(subject_id) ON DELETE CASCADE,
    title VARCHAR(200) NOT NULL,
    description TEXT,
    due_date DATE
);

-- ===========================================
--   TABLE: Grades (оцінки студентів)
-- ===========================================
CREATE TABLE Grades (
    grade_id SERIAL PRIMARY KEY,
    student_id INT REFERENCES Students(student_id) ON DELETE CASCADE,
    task_id INT REFERENCES Tasks(task_id) ON DELETE CASCADE,
    grade_value INT CHECK (grade_value BETWEEN 0 AND 100),
    comment TEXT
);
