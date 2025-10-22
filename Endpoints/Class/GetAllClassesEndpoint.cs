using ClassAPI.Data;
using ClassAPI.Dtos.Requests;
using ClassAPI.Dtos.Responses;
using ClassAPI.Common;
using FastEndpoints;

namespace ClassAPI.Endpoints.Class
{

    public class GetAllClassesEndpoint : Endpoint<GetAllClassesRequest, GlobalResponse<GetAllClassesResponse>>
    {
        public override void Configure()
        {
            Get("/api/classes");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetAllClassesRequest req, CancellationToken ct)
        {
            var classes = InMemoryDatabase.Classes.Values.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(req.Name))
            {
                classes = classes.Where(c => c.Name.Contains(req.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(req.Teacher))
            {
                classes = classes.Where(c => c.Teacher.Contains(req.Teacher, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = classes.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageSize);

            // Apply pagination
            var paginatedClasses = classes
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();

            var data = new GetAllClassesResponse
            {
                Classes = paginatedClasses,
                TotalCount = totalCount,
                Page = req.Page,
                PageSize = req.PageSize,
                TotalPages = totalPages
            };

            await SendAsync(new GlobalResponse<GetAllClassesResponse>
            {
                IsSuccess = true,
                Message = "Classes retrieved successfully",
                Data = data
            }, 200, ct);
        }
    }
}
