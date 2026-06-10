using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tutor.Api
{
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected string UserEmail => User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        protected string GetClaimValue(string claimType) => User.FindFirst(claimType)?.Value ?? string.Empty;
    }
}
