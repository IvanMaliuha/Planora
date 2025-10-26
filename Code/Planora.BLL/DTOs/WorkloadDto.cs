namespace Planora.BLL.DTOs
{
    // Цей DTO містить агреговані дані, необхідні для формування Workload
    // та для подальшої генерації розкладу.
    public class WorkloadDto
    {
        // Ключі для збереження в DAL Workload (Якщо це окрема таблиця для збереження)
        public int UserId { get; set; } // TeacherId
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public int Duration { get; set; } // Тривалість годин на предмет

        // Деталі для звітів/консольної програми
        public string TeacherName { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public string SubjectType { get; set; } = "";
        
        // Деталі аудиторії (для кращого підбору)
        public string ClassroomNumber { get; set; } = ""; 
        public string ClassroomBuilding { get; set; } = "";
        public int ClassroomCapacity { get; set; }
        public bool HasProjector { get; set; }
        public bool HasComputers { get; set; }
    }
}