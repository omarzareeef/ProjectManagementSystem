using FluentValidation;
using PMS.Application.DTOs.Project;

namespace PMS.Application.Validators.Project;

public class UpdateProjectValidator : AbstractValidator<UpdateProjectDTO>
{
    public UpdateProjectValidator()
    {
        Include(new BaseProjectValidator());

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identifier is required.");
    }
}
