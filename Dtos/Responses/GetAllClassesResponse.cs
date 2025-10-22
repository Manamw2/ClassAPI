namespace ClassAPI.Dtos.Responses
{
    public class GetAllClassesResponse
    {
        public List<Models.Entities.Class> Classes { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
