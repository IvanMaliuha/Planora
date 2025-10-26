namespace Planora.BLL.DTOs
{
    public class AddStudentDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Faculty { get; set; }
        public string GroupName { get; set; } // Назва групи, а не Id, для зручності введення
    }
}