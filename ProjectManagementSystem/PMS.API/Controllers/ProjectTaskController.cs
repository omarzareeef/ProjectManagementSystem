using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.LookUp;
using PMS.Application.DTOs.Pagination;
using PMS.Application.DTOs.ProjectTask;
using PMS.Application.DTOs.Responses;
using PMS.Application.Services.ProjectTaskService;

namespace PMS.API.Controllers;

[Authorize]
public class ProjectTaskController : AppBaseController
{
    private readonly IProjectTaskService _projectTaskService;

    public ProjectTaskController(IProjectTaskService projectTaskService)
    {
        _projectTaskService = projectTaskService;
    }

    [HttpGet("GetByProjectId")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProjectTaskDTO>>>> GetByProjectId(
        [FromQuery] ProjectTasksByProjectIdPagedRequestDTO requestDTO, 
        CancellationToken cancellationToken = default)
    {
        return Ok(await _projectTaskService.GetByProjectId(requestDTO, UserId, cancellationToken));
    }

    [HttpPost("Add")]
    public async Task<ActionResult<ApiResponse<bool>>> Add(
        CreateProjectTaskDTO requestDTO, 
        CancellationToken cancellationToken = default)
    {
        return StatusCode(StatusCodes.Status201Created, await _projectTaskService.Add(requestDTO, UserId, cancellationToken));
    }

    [HttpPut("Update")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(
        UpdateProjectTaskDTO requestDTO, 
        CancellationToken cancellationToken = default)
    {
        return Ok(await _projectTaskService.Update(requestDTO, UserId, cancellationToken));
    }

    [HttpDelete("Delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return Ok(await _projectTaskService.Delete(id, UserId, cancellationToken));
    }
}
