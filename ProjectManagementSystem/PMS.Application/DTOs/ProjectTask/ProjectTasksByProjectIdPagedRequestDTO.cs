using PMS.Application.DTOs.Pagination;

namespace PMS.Application.DTOs.ProjectTask;

public class ProjectTasksByProjectIdPagedRequestDTO : PagedRequest
{
    public Guid ProjectId { get; set; }
}
