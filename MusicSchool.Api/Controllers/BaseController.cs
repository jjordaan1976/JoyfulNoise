using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MusicSchool.Api
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
