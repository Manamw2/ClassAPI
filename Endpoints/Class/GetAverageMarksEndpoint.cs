using ClassAPI.Data;
using ClassAPI.Dtos.Responses;
using ClassAPI.Common;
using FastEndpoints;

namespace ClassAPI.Endpoints.Class
{


    public class GetAverageMarksEndpoint : EndpointWithoutRequest<GlobalResponse<AverageMarksResponse>>
    {
        public override void Configure()
        {
            Get("/api/classes/{classId}/average-marks");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var classId = Route<int>("classId");

            // Check if class exists
            if (!InMemoryDatabase.Classes.TryGetValue(classId, out var classEntity))
            {
                await SendAsync(new GlobalResponse<AverageMarksResponse>
                {
                    IsSuccess = false,
                    Message = "Class not found"
                }, 404, ct);
                return;
            }

            // Get all marks for this class
            var classMarks = InMemoryDatabase.Marks.Values
                .Where(m => m.ClassId == classId && !m.IsArchived)
                .ToList();

            if (!classMarks.Any())
            {
                await SendAsync(new GlobalResponse<AverageMarksResponse>
                {
                    IsSuccess = false,
                    Message = "No marks recorded for this class"
                }, 404, ct);
                return;
            }

            var averageTotalMark = classMarks.Average(m => m.TotalMark);
            var studentCount = classMarks.Select(m => m.StudentId).Distinct().Count();

            var data = new AverageMarksResponse
            {
                ClassId = classId,
                ClassName = classEntity.Name,
                AverageTotalMark = averageTotalMark,
                StudentCount = studentCount
            };

            await SendAsync(new GlobalResponse<AverageMarksResponse>
            {
                IsSuccess = true,
                Message = "Average marks retrieved successfully",
                Data = data
            }, 200, ct);
        }
    }
}

