using ClassAPI.Data;
using ClassAPI.Models.Entities;
using ClassAPI.Common;
using FastEndpoints;
using FluentValidation;

namespace ClassAPI.Endpoints.Students
{
    public record CreateStudentRequest(string FirstName, string LastName, int Age);

    public class CreateStudentValidator : Validator<CreateStudentRequest>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Age).GreaterThan(0);
        }
    }

    public class CreateStudentEndpoint : Endpoint<CreateStudentRequest, GlobalResponse<Student>>
    {
        public override void Configure()
        {
            Post("/api/students");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateStudentRequest req, CancellationToken ct)
        {
            var student = new Student
            {
                Id = InMemoryDatabase.StudentId++,
                FirstName = req.FirstName,
                LastName = req.LastName,
                Age = req.Age
            };
            InMemoryDatabase.Students[student.Id] = student;
            await SendAsync(new GlobalResponse<Student>
            {
                IsSuccess = true,
                Message = "Student created successfully",
                Data = student
            }, 201, ct);
        }
    }
}
