namespace ClassAPI.Dtos.Requests
{
    public class GetAllMarksRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsArchived { get; set; } = null; // null = all, true = archived only, false = active only
    }
}
