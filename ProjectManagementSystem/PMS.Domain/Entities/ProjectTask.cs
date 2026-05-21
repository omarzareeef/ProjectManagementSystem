using PMS.Domain.Enums;

namespace PMS.Domain.Entities;

public class ProjectTask : BaseEntity
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public ProjectTaskStatus Status { get; set; }

    public DateTime DueDate { get; set; }

    public ProjectTaskPriority Priority { get; set; }

    public Guid ProjectId { get; set; }


    public User CreatedBy { get; set; } = null!;

    public Project Project { get; set; } = null!;
}
