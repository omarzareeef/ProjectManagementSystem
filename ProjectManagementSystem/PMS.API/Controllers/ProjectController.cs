using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.LookUp;
using PMS.Application.DTOs.Pagination;
using PMS.Application.DTOs.Project;
using PMS.Application.DTOs.Responses;
using PMS.Application.Services.ProjectService;

namespace PMS.API.Controllers
{
    [Authorize]
    public class ProjectController : AppBaseController
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("Get")]
        public async Task<ActionResult<ApiResponse<PagedResult<ProjectDTO>>>> Get(
            [FromQuery] PagedRequest requestDTO, 
            CancellationToken cancellationToken = default)
        {
            return Ok(await _projectService.Get(requestDTO, UserId, cancellationToken));
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<ApiResponse<ProjectDTO>>> GetById(
            Guid id, 
            CancellationToken cancellationToken = default)
        {
            return Ok(await _projectService.GetById(id, UserId, cancellationToken));
        }

        [HttpGet("GetList")]
        public async Task<ActionResult<ApiResponse<List<LookUpDTO>>>> GetList(
            string? name, 
            CancellationToken cancellationToken = default)
        {
            return Ok(await _projectService.GetList(name, UserId, cancellationToken));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<bool>>> Add(
            CreateProjectDTO projectDTO, 
            CancellationToken cancellationToken = default)
        {
            return StatusCode(StatusCodes.Status201Created, await _projectService.Add(projectDTO, UserId, cancellationToken));
        }

        [HttpPut("Update")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(
            UpdateProjectDTO projectDTO, 
            CancellationToken cancellationToken = default)
        {
            return Ok(await _projectService.Update(projectDTO, UserId, cancellationToken));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            return Ok(await _projectService.Delete(id, UserId, cancellationToken));
        }
    }
}
