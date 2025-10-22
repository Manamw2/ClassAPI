using ClassAPI.Data;
using ClassAPI.Models.Entities;
using ClassAPI.Common;
using FastEndpoints;
using FluentValidation;

namespace ClassAPI.Endpoints.Mark
{
    public record CreateMarkRequest(int StudentId, int ClassId, decimal ExamMark, decimal AssignmentMark);

    public class CreateMarkValidator : Validator<CreateMarkRequest>
    {
        public CreateMarkValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0).WithMessage("Student ID must be greater than 0");
            RuleFor(x => x.ClassId).GreaterThan(0).WithMessage("Class ID must be greater than 0");
            RuleFor(x => x.ExamMark)
                .GreaterThanOrEqualTo(0).WithMessage("Exam mark must be between 0 and 100")
                .LessThanOrEqualTo(100).WithMessage("Exam mark must be between 0 and 100");
            RuleFor(x => x.AssignmentMark)
                .GreaterThanOrEqualTo(0).WithMessage("Assignment mark must be between 0 and 100")
                .LessThanOrEqualTo(100).WithMessage("Assignment mark must be between 0 and 100");
        }
    }

    public class CreateMarkEndpoint : Endpoint<CreateMarkRequest, GlobalResponse<Models.Entities.Mark>>
    {
        public override void Configure()
        {
            Post("/api/marks");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateMarkRequest req, CancellationToken ct)
        {
            // Check if student exists
            if (!InMemoryDatabase.Students.ContainsKey(req.StudentId))
            {
                await SendAsync(new GlobalResponse<Models.Entities.Mark>
                {
                    IsSuccess = false,
                    Message = "Student not found"
                }, 404, ct);
                return;
            }

            // Check if class exists
            if (!InMemoryDatabase.Classes.ContainsKey(req.ClassId))
            {
                await SendAsync(new GlobalResponse<Models.Entities.Mark>
                {
                    IsSuccess = false,
                    Message = "Class not found"
                }, 404, ct);
                return;
            }

            // Check if student is enrolled in the class
            var enrollment = InMemoryDatabase.Enrollments.Values
                .FirstOrDefault(e => e.StudentId == req.StudentId && e.ClassId == req.ClassId);

            if (enrollment == null)
            {
                await SendAsync(new GlobalResponse<Models.Entities.Mark>
                {
                    IsSuccess = false,
                    Message = "Student is not enrolled in this class"
                }, 400, ct);
                return;
            }



            var mark = new Models.Entities.Mark
            {
                Id = InMemoryDatabase.MarkId++,
                StudentId = req.StudentId,
                ClassId = req.ClassId,
                ExamMark = req.ExamMark,
                AssignmentMark = req.AssignmentMark
            };

            InMemoryDatabase.Marks[mark.Id] = mark;

            await SendAsync(new GlobalResponse<Models.Entities.Mark>
            {
                IsSuccess = true,
                Message = "Mark created successfully",
                Data = mark
            }, 201, ct);
        }
    }
}