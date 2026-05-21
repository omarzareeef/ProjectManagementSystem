using FluentValidation;
using PMS.Application.DTOs.ProjectTask;

namespace PMS.Application.Validators.ProjectTask;

public class UpdateProjectTaskValidator : AbstractValidator<UpdateProjectTaskDTO>
{
    public UpdateProjectTaskValidator()
    {
        Include(new BaseProjectTaskValidator());

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identifier is required.");
    }
}
