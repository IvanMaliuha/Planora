namespace Planora.BLL.DTOs
{
    public class AddSubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Requirements { get; set; }
        public int? Duration { get; set; }
    }
}