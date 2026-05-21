using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PMS.Application.DTOs.LookUp;
using PMS.Application.DTOs.Pagination;
using PMS.Application.DTOs.Project;
using PMS.Application.DTOs.Responses;
using PMS.Application.Exceptions;
using PMS.Application.Interfaces.Persistence;
using PMS.Domain.Entities;

namespace PMS.Application.Services.ProjectService;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IRepository<Project> _projectRepository;

    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IUnitOfWork unitOfWork,
        IRepository<Project> projectRepository,
        ILogger<ProjectService> logger)
    {
        _unitOfWork = unitOfWork;
        _projectRepository = projectRepository;
        _logger = logger;
    }


    public async Task<ApiResponse<PagedResult<ProjectDTO>>> Get(PagedRequest requestDTO, Guid userId, CancellationToken cancellationToken = default)
    {
        var query = _projectRepository.GetQueryable();

        var result = await _projectRepository.GetPagedAsync(
            query,
            requestDTO.PageNumber,
            requestDTO.PageSize,
            project => new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description
            },
            requestDTO.OrderBy,
            requestDTO.IsDescending,
            cancellationToken);
        return ApiResponse<PagedResult<ProjectDTO>>.Success(result);
    }

    public async Task<ApiResponse<ProjectDTO>> GetById(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken) ??
            throw new NotFoundException("Project Not Found");

        var result = new ProjectDTO
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description
        };

        return ApiResponse<ProjectDTO>.Success(result);
    }

    public async Task<ApiResponse<List<LookUpDTO>>> GetList(string? name, Guid userId, CancellationToken cancellationToken = default)
    {
        var query = _projectRepository.GetQueryable(x => x.CreatedById == userId);

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(x => x.Name.Contains(name));
        }

        var result = await query
            .Select(x => new LookUpDTO
            {
                Id = x.Id,
                Name = x.Name,
            })
            .Take(Constants.MaxDropdownListSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<LookUpDTO>>.Success(result);
    }

    public async Task<ApiResponse<bool>> Add(CreateProjectDTO ProjectDTO, Guid userId, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = ProjectDTO.Name,
            Description = ProjectDTO.Description,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _projectRepository.AddAsync(project, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save Project '{projectName}' to the database.", project.Name);

            throw;
        }

        return ApiResponse<bool>.Success(true, "Project Added Successfully");
    }

    public async Task<ApiResponse<bool>> Update(UpdateProjectDTO ProjectDTO, Guid userId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository
            .GetQueryable(r =>
                r.Id == ProjectDTO.Id &&
                r.CreatedById == userId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Project Not Found");

        project.Name = ProjectDTO.Name;
        project.Description = ProjectDTO.Description;
        project.ModifiedById = userId;
        project.ModifiedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Update Project '{projectName}' to the database.", project.Name);

            throw;
        }

        return ApiResponse<bool>.Success(true, "Project Updated Successfully");
    }

    public async Task<ApiResponse<bool>> Delete(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository
            .GetQueryable(r =>
                r.Id == id &&
                r.CreatedById == userId)
            .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Project Not Found");

        try
        {
            _projectRepository.Delete(project);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully user: {userId}, Deleted the Project: {projectId}", userId, project.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to Delete Project: '{projectName}', By user: {userId}.", project.Name, userId);

            throw;
        }

        return ApiResponse<bool>.Success(true, "Project Deleted Successfully");
    }

    public async Task<bool> IsExist(Guid id, CancellationToken cancellationToken = default)
    {
        return await _projectRepository
            .GetQueryable()
            .AnyAsync(x => x.Id == id);
    }
}
