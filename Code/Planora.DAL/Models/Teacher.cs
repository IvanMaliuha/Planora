namespace Planora.DAL.Models
{
    public class Teacher : User
    {
        public string Faculty { get; set; } = "";
        public string? Position { get; set; }

        public ICollection<TeachingAssignment>? TeachingAssignments { get; set; }
        public ICollection<Workload>? Workloads { get; set; }
    }
}