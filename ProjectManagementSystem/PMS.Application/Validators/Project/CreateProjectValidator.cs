using FluentValidation;
using PMS.Application.DTOs.Project;

namespace PMS.Application.Validators.Project;

public class CreateProjectValidator : AbstractValidator<CreateProjectDTO>
{
    public CreateProjectValidator()
    {
        Include(new BaseProjectValidator());
    }
}
