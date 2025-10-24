-- ===========================================
--   PLANORA DATABASE STRUCTURE (PostgreSQL)
-- ===========================================

-- 1. Users
CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100),
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('admin', 'teacher', 'student'))
);

-- 2. Administrator
CREATE TABLE Administrator (
    user_id INT PRIMARY KEY,
    FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE
);

-- 3. Teachers
CREATE TABLE Teachers (
    user_id INT PRIMARY KEY,
    faculty VARCHAR(100) NOT NULL,
    position VARCHAR(100),
    FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE
);

-- 4. Groups (має бути перед Students!)
CREATE TABLE Groups (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    faculty VARCHAR(100) NOT NULL,
    student_count INT DEFAULT 0
);

-- 5. Students
CREATE TABLE Students (
    user_id INT PRIMARY KEY,
    group_id INT,
    faculty VARCHAR(100) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES Groups(id) ON DELETE SET NULL
);

-- 6. Subjects
CREATE TABLE Subjects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    type VARCHAR(50) NOT NULL,
    requirements TEXT,
    duration INT
);

-- 7. Classrooms
CREATE TABLE Classrooms (
    id SERIAL PRIMARY KEY,
    number VARCHAR(50) NOT NULL,
    building VARCHAR(50) NOT NULL,
    capacity INT NOT NULL,
    faculty VARCHAR(100) NOT NULL,
    has_computers BOOLEAN DEFAULT FALSE,
    has_projector BOOLEAN DEFAULT FALSE
);

-- 8. Schedule
CREATE TABLE Schedule (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    subject_id INT NOT NULL,
    group_id INT NOT NULL,
    classroom_id INT NOT NULL,
    day_of_week INT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    week_type VARCHAR(10) DEFAULT 'both',
    FOREIGN KEY (user_id) REFERENCES Users(id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES Groups(id) ON DELETE CASCADE,
    FOREIGN KEY (classroom_id) REFERENCES Classrooms(id) ON DELETE CASCADE
);

-- 9. TeachingAssignment
CREATE TABLE TeachingAssignment (
    assignment_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    subject_id INT NOT NULL,
    hours INT,
    FOREIGN KEY (user_id) REFERENCES Teachers(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);

-- 10. GroupDisciplineList
CREATE TABLE GroupDisciplineList (
    list_id SERIAL PRIMARY KEY,
    group_id INT NOT NULL,
    subject_id INT NOT NULL,
    hours INT,
    FOREIGN KEY (group_id) REFERENCES Groups(id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);

-- 11. Workload
CREATE TABLE Workload (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    subject_id INT NOT NULL,
    group_id INT NOT NULL,
    duration INT,
    FOREIGN KEY (user_id) REFERENCES Teachers(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES Groups(id) ON DELETE CASCADE
);
