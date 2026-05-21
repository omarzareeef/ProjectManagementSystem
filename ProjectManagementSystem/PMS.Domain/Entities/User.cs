using Microsoft.AspNetCore.Identity;

namespace PMS.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Projects = new HashSet<Project>();
        ProjectTasks = new HashSet<ProjectTask>();
    }


    public ICollection<Project> Projects { get; set; }

    public ICollection<ProjectTask> ProjectTasks { get; set; }
}
