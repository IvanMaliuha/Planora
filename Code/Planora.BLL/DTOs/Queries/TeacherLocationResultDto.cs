// Planora.BLL/DTOs/Queries/TeacherLocationResultDto.cs

namespace Planora.BLL.DTOs.Queries
{
    public class TeacherLocationResultDto
    {
        public bool IsCurrentlyTeaching { get; set; }
        
        // Якщо викладає:
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ClassroomNumber { get; set; } = "";
        public string Building { get; set; } = "";
        
        // Якщо не викладає, або завжди:
        public string TeacherFaculty { get; set; } = "";
        public string Message { get; set; } = ""; // Детальніше повідомлення (наприклад, "Пара закінчилася", "Викладає пізніше")
    }
}