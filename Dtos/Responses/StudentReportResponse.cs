namespace ClassAPI.Dtos.Responses
{
    public class StudentReportResponse
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public List<ClassReportItem> Classes { get; set; } = new();
        public decimal OverallAverageMark { get; set; }
    }

    public class ClassReportItem
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Teacher { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public decimal? ExamMark { get; set; }
        public decimal? AssignmentMark { get; set; }
        public decimal? TotalMark { get; set; }
        public string? GradeCategory { get; set; }
        public int? Ranking { get; set; }
        public PreformanceTrendStatus? PreformanceTrend { get; set; }
    }
    public enum PreformanceTrendStatus
    {
        Stable,
        Improving,
        Declining
    }
}
