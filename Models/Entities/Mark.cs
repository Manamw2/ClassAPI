namespace ClassAPI.Models.Entities
{
    public class Mark
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public decimal ExamMark { get; set; }
        public decimal AssignmentMark { get; set; }
        public decimal TotalMark => ExamMark + AssignmentMark;
        public DateTime AddedAt { get; set; } = DateTime.Now;
        public bool IsArchived { get; set; } = false;
    }
}
