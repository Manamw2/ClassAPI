using ClassAPI.Data;
using ClassAPI.Dtos.Requests;
using ClassAPI.Dtos.Responses;
using ClassAPI.Common;
using FastEndpoints;
using FluentValidation;

namespace ClassAPI.Endpoints.Mark
{
    public class GetAllMarksValidator : Validator<GetAllMarksRequest>
    {
        public GetAllMarksValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0);
            RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        }
    }

    public class GetAllMarksEndpoint : Endpoint<GetAllMarksRequest, GlobalResponse<GetAllMarksResponse>>
    {
        public override void Configure()
        {
            Get("/api/marks");
            AllowAnonymous();
        }

        public override async Task HandleAsync(GetAllMarksRequest req, CancellationToken ct)
        {
            var allMarks = InMemoryDatabase.Marks.Values.AsQueryable();

            // Apply IsArchived filter
            if (req.IsArchived.HasValue)
            {
                allMarks = allMarks.Where(m => m.IsArchived == req.IsArchived.Value);
            }

            var totalCount = allMarks.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / req.PageSize);

            var marks = allMarks
                .OrderByDescending(m => m.AddedAt)
                .Skip((req.Page - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();

            var response = new GetAllMarksResponse
            {
                Marks = marks,
                TotalCount = totalCount,
                Page = req.Page,
                PageSize = req.PageSize,
                TotalPages = totalPages,
                IsArchived = req.IsArchived
            };

            await SendAsync(new GlobalResponse<GetAllMarksResponse>
            {
                IsSuccess = true,
                Message = "Marks retrieved successfully",
                Data = response
            }, 200, ct);
        }
    }
}
