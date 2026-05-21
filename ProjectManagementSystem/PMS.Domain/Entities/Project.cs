namespace PMS.Domain.Entities;

public class Project : BaseEntity
{
    public Project()
    {
        ProjectTasks = new HashSet<ProjectTask>();
    }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }


    public User CreatedBy { get; set; } = null!;

    public ICollection<ProjectTask> ProjectTasks { get; set; }
}
