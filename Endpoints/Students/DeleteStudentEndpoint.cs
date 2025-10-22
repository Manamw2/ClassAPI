using ClassAPI.Data;
using ClassAPI.Common;
using FastEndpoints;

namespace ClassAPI.Endpoints.Students
{
    public class DeleteStudentEndpoint : EndpointWithoutRequest<GlobalResponse<object>>
    {
        public override void Configure()
        {
            Delete("/api/students/{id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var id = Route<int>("id");

            if (!InMemoryDatabase.Students.TryRemove(id, out _))
            {
                await SendAsync(new GlobalResponse<object>
                {
                    IsSuccess = false,
                    Message = "Student not found"
                }, 404, ct);
                return;
            }

            await SendAsync(new GlobalResponse<object>
            {
                IsSuccess = true,
                Message = "Student deleted successfully",
                Data = null
            }, 200, ct);
        }
    }
}
