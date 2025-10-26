// Planora.BLL/DTOs/Queries/FindTeacherLocationQueryDto.cs

namespace Planora.BLL.DTOs.Queries
{
    public class FindTeacherLocationQueryDto
    {
        public string FullName { get; set; } = "";
        public int CurrentDayOfWeek { get; set; } // Наприклад, 1-7
        public TimeOnly CurrentTime { get; set; } // Поточна година
    }
}