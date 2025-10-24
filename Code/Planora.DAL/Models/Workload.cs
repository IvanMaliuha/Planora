namespace Planora.DAL.Models
{
    public class Workload
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public int? Duration { get; set; }

        public Teacher? Teacher { get; set; }
        public Subject? Subject { get; set; }
        public Group? Group { get; set; }
    }
}