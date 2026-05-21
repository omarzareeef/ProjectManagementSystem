using FluentValidation;
using PMS.Application.DTOs.Project;

namespace PMS.Application.Validators.Project;

public class BaseProjectValidator : AbstractValidator<BaseProjectDTO>
{
    public BaseProjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");
    }
}
