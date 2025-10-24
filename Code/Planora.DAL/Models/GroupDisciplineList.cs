namespace Planora.DAL.Models
{
    public class GroupDisciplineList
    {
        public int ListId { get; set; }
        public int GroupId { get; set; }
        public int SubjectId { get; set; }
        public int? Hours { get; set; }

        public Group? Group { get; set; }
        public Subject? Subject { get; set; }
    }
}