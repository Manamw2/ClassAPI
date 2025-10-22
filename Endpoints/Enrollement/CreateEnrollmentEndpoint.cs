using ClassAPI.Data;
using ClassAPI.Common;
using ClassAPI.Models.Entities;
using FastEndpoints;
using FluentValidation;

namespace ClassAPI.Endpoints.Enrollement
{
    public record CreateEnrollmentRequest(int StudentId, int ClassId);

    public class CreateEnrollmentValidator : Validator<CreateEnrollmentRequest>
    {
        public CreateEnrollmentValidator()
        {
            RuleFor(x => x.StudentId).GreaterThan(0);
            RuleFor(x => x.ClassId).GreaterThan(0);
        }
    }

    public class CreateEnrollmentEndpoint : Endpoint<CreateEnrollmentRequest, GlobalResponse<Enrollment>>
    {
        public override void Configure()
        {
            Post("/api/enrollments");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateEnrollmentRequest req, CancellationToken ct)
        {
            // Check if student exists
            if (!InMemoryDatabase.Students.ContainsKey(req.StudentId))
            {
                await SendAsync(new GlobalResponse<Enrollment>
                {
                    IsSuccess = false,
                    Message = "Student not found"
                }, 404, ct);
                return;
            }

            // Check if class exists
            if (!InMemoryDatabase.Classes.ContainsKey(req.ClassId))
            {
                await SendAsync(new GlobalResponse<Enrollment>
                {
                    IsSuccess = false,
                    Message = "Class not found"
                }, 404, ct);
                return;
            }

            // Check for duplicate enrollment
            var existingEnrollment = InMemoryDatabase.Enrollments.Values
                .FirstOrDefault(e => e.StudentId == req.StudentId && e.ClassId == req.ClassId);

            if (existingEnrollment != null)
            {
                await SendAsync(new GlobalResponse<Enrollment>
                {
                    IsSuccess = false,
                    Message = "Student is already enrolled in this class"
                }, 400, ct);
                return;
            }

            // Check if student is already enrolled in 5 classes (maximum limit)
            var studentEnrollments = InMemoryDatabase.Enrollments.Values
                .Count(e => e.StudentId == req.StudentId);

            if (studentEnrollments >= 5)
            {
                await SendAsync(new GlobalResponse<Enrollment>
                {
                    IsSuccess = false,
                    Message = "Student cannot enroll in more than 5 classes. Current enrollment count: " + studentEnrollments
                }, 400, ct);
                return;
            }

            // Check if class has reached maximum capacity (30 students)
            var classEnrollments = InMemoryDatabase.Enrollments.Values
                .Count(e => e.ClassId == req.ClassId);

            if (classEnrollments >= 30)
            {
                await SendAsync(new GlobalResponse<Enrollment>
                {
                    IsSuccess = false,
                    Message = "Class has reached maximum capacity of 30 students. Current enrollment count: " + classEnrollments
                }, 400, ct);
                return;
            }

            var enrollment = new Enrollment
            {
                Id = InMemoryDatabase.EnrollmentId++,
                StudentId = req.StudentId,
                ClassId = req.ClassId,
                EnrollmentDate = DateTime.UtcNow
            };

            InMemoryDatabase.Enrollments[enrollment.Id] = enrollment;

            await SendAsync(new GlobalResponse<Enrollment>
            {
                IsSuccess = true,
                Message = "Enrollment created successfully",
                Data = enrollment
            }, 201, ct);
        }
    }
}

