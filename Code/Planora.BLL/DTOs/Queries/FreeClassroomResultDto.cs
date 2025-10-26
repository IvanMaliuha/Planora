namespace Planora.BLL.DTOs.Queries
{
    public class FreeClassroomResultDto
    {
        public int ClassroomId { get; set; }
        public string Number { get; set; } = "";
        public string Building { get; set; } = "";
        public int Capacity { get; set; }
    }
}