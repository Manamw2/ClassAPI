using ClassAPI.Data;
using ClassAPI.Dtos.Requests;
using ClassAPI.Dtos.Responses;
using ClassAPI.Endpoints.Class;
using ClassAPI.Models.Entities;
using ClassAPI.Common;
using FastEndpoints;

namespace ClassAPI.Endpoints.Students
{

    public class GetAllStudentsEndpoint : Endpoint<GetAllStudentsRequest, GlobalResponse<GetAllStudentsResponse>>
    {
        public override void Configure()
        {
            Get("/api/students");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetAllStudentsRequest req, CancellationToken ct)
        {
            var students = InMemoryDatabase.Students.Values.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(req.Name))
            {
                students = students.Where(s => 
                    s.FirstName.Contains(req.Name, StringComparison.OrdinalIgnoreCase) ||
                    s.LastName.Contains(req.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (req.Age.HasValue)
            {
                students = students.Where(s => s.Age == req.Age.Value);
            }

            var totalCount = students.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageSize);

            // Apply pagination
            var paginatedStudents = students
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();

            var data = new GetAllStudentsResponse
            {
                Students = paginatedStudents,
                TotalCount = totalCount,
                Page = req.Page,
                PageSize = req.PageSize,
                TotalPages = totalPages
            };

            await SendAsync(new GlobalResponse<GetAllStudentsResponse>
            {
                IsSuccess = true,
                Message = "Students retrieved successfully",
                Data = data
            }, 200, ct);
        }
    }
}
