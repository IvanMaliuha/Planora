namespace Planora.BLL.DTOs
{
    public class AddTeachingAssignmentDto
    {
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int? Hours { get; set; }

        public AddTeacherDto? Teacher { get; set; }
        public AddSubjectDto? Subject { get; set; }
    }
}