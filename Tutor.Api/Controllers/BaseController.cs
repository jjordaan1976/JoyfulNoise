using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tutor.Api
{
    [Authorize]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected string UserEmail =>
            User.FindFirst(ClaimTypes.Email)?.Value ??
            User.FindFirst("email")?.Value ??
            string.Empty;
    }
}
