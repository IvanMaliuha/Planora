namespace Planora.DAL.Models
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Number { get; set; } = "";
        public string Building { get; set; } = "";
        public int Capacity { get; set; }
        public string Faculty { get; set; } = "";
        public bool HasComputers { get; set; }
        public bool HasProjector { get; set; }
    }
}