-- ===========================================
--   CREATE DATABASE STRUCTURE FOR DESKTOP APP
-- ===========================================

-- ===========================================
-- 1. Users
-- ===========================================
CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    full_name VARCHAR(100),
    email VARCHAR(100),
    login VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) CHECK (role IN ('admin', 'teacher', 'student')) NOT NULL
);

-- ===========================================
-- 2. Administrator
-- ===========================================
CREATE TABLE Administrator (
    user_id INT PRIMARY KEY,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

-- ===========================================
-- 3. Teachers
-- ===========================================
CREATE TABLE Teachers (
    user_id INT PRIMARY KEY,
    department VARCHAR(100) NOT NULL,
    position VARCHAR(100),
    email VARCHAR(100),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

-- ===========================================
-- 4. Groups
-- ===========================================
CREATE TABLE Groups (
    group_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    amount_students INT,
    faculty VARCHAR(100)
);

-- ===========================================
-- 5. Students
-- ===========================================
CREATE TABLE Students (
    user_id INT PRIMARY KEY,
    group_id INT,
    faculty VARCHAR(100) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES Groups(group_id) ON DELETE SET NULL
);

-- ===========================================
-- 6. Subjects
-- ===========================================
CREATE TABLE Subjects (
    subject_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    credits INT,
    semester INT
);

-- ===========================================
-- 7. Audience (Classrooms)
-- ===========================================
CREATE TABLE Audience (
    audience_id SERIAL PRIMARY KEY,
    number VARCHAR(50) NOT NULL,
    corps VARCHAR(50) NOT NULL,
    capacity INT,
    has_projector BOOLEAN DEFAULT FALSE,
    has_computers BOOLEAN DEFAULT FALSE
);

-- ===========================================
-- 8. Schedule
-- ===========================================
CREATE TABLE Schedule (
    schedule_id SERIAL PRIMARY KEY,
    subject_id INT NOT NULL,
    teacher_id INT NOT NULL,
    group_id INT NOT NULL,
    audience_id INT NOT NULL,
    day VARCHAR(20) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    lesson_type VARCHAR(50),
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE,
    FOREIGN KEY (teacher_id) REFERENCES Teachers(user_id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES Groups(group_id) ON DELETE CASCADE,
    FOREIGN KEY (audience_id) REFERENCES Audience(audience_id) ON DELETE CASCADE
);

-- ===========================================
-- 9. RoomSearch
-- ===========================================
CREATE TABLE RoomSearch (
    search_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    corps VARCHAR(50),
    type VARCHAR(50),
    capacity INT,
    has_computers BOOLEAN DEFAULT FALSE,
    has_projector BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

-- ===========================================
-- 10. TeachingAssignment
-- ===========================================
CREATE TABLE TeachingAssignment (
    assignment_id SERIAL PRIMARY KEY,
    teacher_id INT NOT NULL,
    subject_id INT NOT NULL,
    group_id INT NOT NULL,
    hours INT,
    requirements TEXT,
    FOREIGN KEY (teacher_id) REFERENCES Teachers(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES Groups(group_id) ON DELETE CASCADE
);

-- ===========================================
-- 11. GroupDisciplineList (для розширення)
-- ===========================================
CREATE TABLE GroupDisciplineList (
    list_id SERIAL PRIMARY KEY,
    group_id INT NOT NULL,
    subject_id INT NOT NULL,
    FOREIGN KEY (group_id) REFERENCES Groups(group_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE
);

-- ===========================================
-- 12. Workload
-- ===========================================
CREATE TABLE Workload (
    workload_id SERIAL PRIMARY KEY,
    group_id INT NOT NULL,
    teacher_id INT NOT NULL,
    subject_id INT NOT NULL,
    audience_id INT NOT NULL,
    day VARCHAR(20),
    start_time TIME,
    end_time TIME,
    hours_needed INT,
    FOREIGN KEY (group_id) REFERENCES Groups(group_id) ON DELETE CASCADE,
    FOREIGN KEY (teacher_id) REFERENCES Teachers(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE,
    FOREIGN KEY (audience_id) REFERENCES Audience(audience_id) ON DELETE CASCADE
);