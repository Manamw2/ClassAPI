using ClassAPI.Data;
using ClassAPI.Common;
using FastEndpoints;
using FluentValidation;

namespace ClassAPI.Endpoints.Class
{
    public record CreateClassRequest(string Name, string Teacher, string Description);

    public class CreateClassValidator : Validator<CreateClassRequest>
    {
        public CreateClassValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Teacher).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }

    public class CreateClassEndpoint : Endpoint<CreateClassRequest, GlobalResponse<Models.Entities.Class>>
    {
        public override void Configure()
        {
            Post("/api/classes");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CreateClassRequest req, CancellationToken ct)
        {
            var classEntity = new Models.Entities.Class
            {
                Id = InMemoryDatabase.ClassId++,
                Name = req.Name,
                Teacher = req.Teacher,
                Description = req.Description
            };

            InMemoryDatabase.Classes[classEntity.Id] = classEntity;

            await SendAsync(new GlobalResponse<Models.Entities.Class>
            {
                IsSuccess = true,
                Message = "Class created successfully",
                Data = classEntity
            }, 201, ct);
        }
    }
}
