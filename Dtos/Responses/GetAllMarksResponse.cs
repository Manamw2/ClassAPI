using ClassAPI.Models.Entities;

namespace ClassAPI.Dtos.Responses
{
    public class GetAllMarksResponse
    {
        public List<Mark> Marks { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool? IsArchived { get; set; }
    }
}
