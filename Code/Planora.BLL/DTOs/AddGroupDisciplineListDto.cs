namespace Planora.BLL.DTOs
{
    public class AddGroupDisciplineListDto
    {
        public int ListId { get; set; }
        public int GroupId { get; set; }
        public int SubjectId { get; set; }
        public int? Hours { get; set; }

        public AddGroupDto? Group { get; set; }
        public AddSubjectDto? Subject { get; set; }
    }
}