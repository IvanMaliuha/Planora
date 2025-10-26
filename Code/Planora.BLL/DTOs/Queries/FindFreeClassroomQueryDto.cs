namespace Planora.BLL.DTOs.Queries
{
    public class FindFreeClassroomQueryDto
    {
        public int DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public string Building { get; set; } = "";
        public bool NeedsComputers { get; set; }
        public bool NeedsProjector { get; set; }
        public int RequiredCapacity { get; set; }
    }
}