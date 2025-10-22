using ClassAPI.Data;
using ClassAPI.Models.Entities;
using ClassAPI.Common;
using FastEndpoints;
using FluentValidation;

namespace ClassAPI.Endpoints.Students
{
    public record UpdateStudentRequest(int Id, string FirstName, string LastName, int Age);

    public class UpdateStudentValidator : Validator<UpdateStudentRequest>
    {
        public UpdateStudentValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Age).GreaterThan(0);
        }
    }

    public class UpdateStudentEndpoint : Endpoint<UpdateStudentRequest, GlobalResponse<Student>>
    {
        public override void Configure()
        {
            Put("/api/students/{id}");
            AllowAnonymous();
        }

        public override async Task HandleAsync(UpdateStudentRequest req, CancellationToken ct)
        {
            var id = Route<int>("id");

            if (!InMemoryDatabase.Students.TryGetValue(id, out var student))
            {
                await SendAsync(new GlobalResponse<Student>
                {
                    IsSuccess = false,
                    Message = "Student not found"
                }, 404, ct);
                return;
            }

            student.FirstName = req.FirstName;
            student.LastName = req.LastName;
            student.Age = req.Age;

            InMemoryDatabase.Students[id] = student;

            await SendAsync(new GlobalResponse<Student>
            {
                IsSuccess = true,
                Message = "Student updated successfully",
                Data = student
            }, 200, ct);
        }
    }
}
