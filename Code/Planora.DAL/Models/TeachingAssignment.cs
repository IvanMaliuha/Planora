namespace Planora.DAL.Models
{
    public class TeachingAssignment
    {
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int? Hours { get; set; }

        public Teacher? Teacher { get; set; }
        public Subject? Subject { get; set; }
    }
}