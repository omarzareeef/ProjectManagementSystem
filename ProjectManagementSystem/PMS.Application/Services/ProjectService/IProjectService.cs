using PMS.Application.DTOs.LookUp;
using PMS.Application.DTOs.Pagination;
using PMS.Application.DTOs.Project;
using PMS.Application.DTOs.Responses;

namespace PMS.Application.Services.ProjectService;

public interface IProjectService
{
    Task<ApiResponse<PagedResult<ProjectDTO>>> Get(
        PagedRequest requestDTO,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<ProjectDTO>> GetById(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<LookUpDTO>>> GetList(string? name, Guid userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> Add(CreateProjectDTO ProjectDTO, Guid userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> Update(UpdateProjectDTO ProjectDTO, Guid userId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> Delete(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> IsExist(Guid id, CancellationToken cancellationToken = default);
}
