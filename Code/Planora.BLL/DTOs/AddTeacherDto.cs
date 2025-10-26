namespace Planora.BLL.DTOs
{
    // Містить вхідні дані для створення нового викладача
    public class AddTeacherDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Faculty { get; set; }
        public string Position { get; set; }
    }
}