using PMS.Domain.Enums;

namespace PMS.Application.DTOs.ProjectTask;

public class BaseProjectTaskDTO
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ProjectTaskStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public ProjectTaskPriority Priority { get; set; }

    public Guid ProjectId { get; set; }
}
