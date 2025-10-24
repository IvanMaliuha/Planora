namespace Planora.DAL.Models
{
    public class Student : User
    {
        public int? GroupId { get; set; }
        public string Faculty { get; set; } = "";

        public Group? Group { get; set; }
    }
}