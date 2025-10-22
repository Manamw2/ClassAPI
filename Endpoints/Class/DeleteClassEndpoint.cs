using ClassAPI.Data;
using ClassAPI.Common;
using FastEndpoints;

namespace ClassAPI.Endpoints.Class
{
    public class DeleteClassEndpoint : EndpointWithoutRequest<GlobalResponse<object>>
    {
        public override void Configure()
        {
            Delete("/api/classes/{id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<int>("id");

            if (!InMemoryDatabase.Classes.ContainsKey(id))
            {
                await SendAsync(new GlobalResponse<object>
                {
                    IsSuccess = false,
                    Message = "Class not found"
                }, 404, ct);
                return;
            }

            // Check if class has enrolled students
            var enrolledStudents = InMemoryDatabase.Enrollments.Values
                .Count(e => e.ClassId == id);

            if (enrolledStudents > 0)
            {
                await SendAsync(new GlobalResponse<object>
                {
                    IsSuccess = false,
                    Message = $"Cannot delete class. There are {enrolledStudents} students currently enrolled. Please unenroll all students first."
                }, 400, ct);
                return;
            }

            InMemoryDatabase.Classes.TryRemove(id, out _);

            await SendAsync(new GlobalResponse<object>
            {
                IsSuccess = true,
                Message = "Class deleted successfully",
                Data = null
            }, 200, ct);
        }
    }
}
