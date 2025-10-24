namespace Planora.DAL.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Faculty { get; set; } = "";
        public int StudentCount { get; set; }

        public ICollection<Student>? Students { get; set; }
        public ICollection<GroupDisciplineList>? DisciplineLists { get; set; }
    }
}