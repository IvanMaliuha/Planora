namespace Planora.DAL.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public int ClassroomId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string WeekType { get; set; } = "both";

        public User? User { get; set; }
        public Subject? Subject { get; set; }
        public Group? Group { get; set; }
        public Classroom? Classroom { get; set; }
    }
}