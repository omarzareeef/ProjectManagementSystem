using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PMS.Application.DTOs.LookUp;
using PMS.Application.DTOs.Pagination;
using PMS.Application.DTOs.ProjectTask;
using PMS.Application.DTOs.Responses;
using PMS.Application.Exceptions;
using PMS.Application.Interfaces.Persistence;
using PMS.Application.Services.ProjectService;
using PMS.Domain.Entities;

namespace PMS.Application.Services.ProjectTaskService;

public class ProjectTaskService : IProjectTaskService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRepository<ProjectTask> _projectTaskRepository;

    private readonly ILogger<ProjectTaskService> _logger;

    private readonly IProjectService _projectService;

    public ProjectTaskService(
        IUnitOfWork unitOfWork,
        IRepository<ProjectTask> projectTaskRepository,
        ILogger<ProjectTaskService> logger,
        IProjectService projectService)
    {
        _unitOfWork = unitOfWork;
        _projectTaskRepository = projectTaskRepository;
        _logger = logger;
        _projectService = projectService;
    }


    public async Task<ApiResponse<PagedResult<ProjectTaskDTO>>> GetByProjectId(ProjectTasksByProjectIdPagedRequestDTO requestDTO, Guid userId, CancellationToken cancellationToken = default)
    {
        var query = _projectTaskRepository
            .GetQueryable(x=>x.ProjectId == requestDTO.ProjectId)
            .Include(x => x.Project);

        var result = await _projectTaskRepository.GetPagedAsync(
            query,
            requestDTO.PageNumber,
            requestDTO.PageSize,
            projectTask => new ProjectTaskDTO
            {
                Id = projectTask.Id,
                ProjectId = projectTask.ProjectId,
                ProjectName = projectTask.Project.Name,
                Title = projectTask.Title,
                Description = projectTask.Description,
                Status = projectTask.Status,
                DueDate = projectTask.DueDate,
                Priority = projectTask.Priority
            },
            requestDTO.OrderBy,
            requestDTO.IsDescending,
            cancellationToken);
        return ApiResponse<PagedResult<ProjectTaskDTO>>.Success(result);
    }

    public async Task<ApiResponse<bool>> Add(CreateProjectTaskDTO projectTaskDTO, Guid userId, CancellationToken cancellationToken = default)
    {
        bool isProjectExist = await _projectService.IsExist(projectTaskDTO.ProjectId, cancellationToken);
        if (!isProjectExist)
        {
            return ApiResponse<bool>.Failure("Project is not available");
        }

        var projectTask = new ProjectTask
        {
            Title = projectTaskDTO.Title,
            Description = projectTaskDTO.Description,
            Status = projectTaskDTO.Status,
            DueDate = projectTaskDTO.DueDate,
            Priority = projectTaskDTO.Priority,
            ProjectId = projectTaskDTO.ProjectId,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _projectTaskRepository.AddAsync(projectTask, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save ProjectTask '{ProjectTaskTitle}' to the database.", projectTask.Title);

            throw;
        }

        return ApiResponse<bool>.Success(true, "ProjectTask Added Successfully");
    }

    public async Task<ApiResponse<bool>> Update(UpdateProjectTaskDTO projectTaskDTO, Guid userId, CancellationToken cancellationToken = default)
    {
        bool isProjectExist = await _projectService.IsExist(projectTaskDTO.ProjectId, cancellationToken);
        if (!isProjectExist)
        {
            return ApiResponse<bool>.Failure("Project is not available");
        }

        var projectTask = await _projectTaskRepository
            .GetQueryable(r =>
                r.Id == projectTaskDTO.Id &&
                r.CreatedById == userId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("ProjectTask Not Found");

        projectTask.Title = projectTaskDTO.Title;
        projectTask.Description = projectTaskDTO.Description;
        projectTask.Status = projectTaskDTO.Status;
        projectTask.DueDate = projectTaskDTO.DueDate;
        projectTask.Priority = projectTaskDTO.Priority;
        projectTask.ProjectId = projectTaskDTO.ProjectId;
        projectTask.ModifiedById = userId;
        projectTask.ModifiedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Update ProjectTask '{ProjectTaskTitle}' to the database.", projectTask.Title);

            throw;
        }

        return ApiResponse<bool>.Success(true, "ProjectTask Updated Successfully");
    }

    public async Task<ApiResponse<bool>> Delete(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var projectTask = await _projectTaskRepository
            .GetQueryable(r =>
                r.Id == id &&
                r.CreatedById == userId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("ProjectTask Not Found");

        try
        {
            _projectTaskRepository.Delete(projectTask);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully user: {userId}, Deleted the ProjectTask: {ProjectTaskId}", userId, projectTask.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Delete ProjectTask: '{ProjectTaskTitle}', By user: {userId}.", projectTask.Title, userId);

            throw;
        }

        return ApiResponse<bool>.Success(true, "ProjectTask Deleted Successfully");
    }
}
