using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppBaseController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(id, out var userGuid) ? userGuid : Guid.Empty;
        }
    }
}
