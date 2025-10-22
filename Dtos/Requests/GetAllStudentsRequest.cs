namespace ClassAPI.Dtos.Requests
{
    public class GetAllStudentsRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Name { get; set; }
        public int? Age { get; set; }
    }
}
