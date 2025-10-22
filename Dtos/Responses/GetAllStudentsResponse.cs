using ClassAPI.Models.Entities;

namespace ClassAPI.Dtos.Responses
{
    public class GetAllStudentsResponse
    {
        public List<Student> Students { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
