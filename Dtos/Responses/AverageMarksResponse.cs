namespace ClassAPI.Dtos.Responses
{
    public class AverageMarksResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal AverageTotalMark { get; set; }
        public int StudentCount { get; set; }
    }
}
