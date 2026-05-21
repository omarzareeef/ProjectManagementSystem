namespace PMS.Application.DTOs.ProjectTask;

public class ProjectTaskDTO : BaseProjectTaskDTO
{
    public Guid Id { get; set; }

    public string ProjectName { get; set; } = null!;
}
