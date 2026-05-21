using FluentValidation;
using PMS.Application.DTOs.ProjectTask;

namespace PMS.Application.Validators.ProjectTask;

public class BaseProjectTaskValidator : AbstractValidator<BaseProjectTaskDTO>
{
    public BaseProjectTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid value.");

        RuleFor(x => x.DueDate)
            .Must(date => date != default).WithMessage("Due date is required.")
            .Must(date => date.Date >= DateTime.UtcNow.Date).WithMessage("Due date cannot be in the past.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid value.");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project is required.");
    }
}
