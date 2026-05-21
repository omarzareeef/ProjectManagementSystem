using PMS.Application.DTOs.LookUp;
using PMS.Application.DTOs.Pagination;
using PMS.Application.DTOs.ProjectTask;
using PMS.Application.DTOs.Responses;

namespace PMS.Application.Services.ProjectTaskService;

public interface IProjectTaskService
{
    Task<ApiResponse<PagedResult<ProjectTaskDTO>>> GetByProjectId(
        ProjectTasksByProjectIdPagedRequestDTO requestDTO,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> Add(CreateProjectTaskDTO ProjectTaskDTO, Guid userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> Update(UpdateProjectTaskDTO ProjectTaskDTO, Guid userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> Delete(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
