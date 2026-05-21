using FluentValidation;
using PMS.Application.DTOs.ProjectTask;

namespace PMS.Application.Validators.ProjectTask;

public class CreateProjectTaskValidator : AbstractValidator<CreateProjectTaskDTO>
{
    public CreateProjectTaskValidator()
    {
        Include(new BaseProjectTaskValidator());
    }
}
