using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected string UserEmail => User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        protected string GetClaimValue(string claimType) => User.FindFirst(claimType)?.Value ?? string.Empty;

        /// <summary>
        /// The single place exceptions are caught, logged, and wrapped in ResponseBase.
        /// Every controller action must be a one-line call to this method.
        /// </summary>
        protected async Task<ResponseBase<T>> Execute<T>(Func<Task<T>> action, ILogger logger, string errorMessage)
        {
            try
            {
                return ResponseBase<T>.Success(await action());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return ResponseBase<T>.Failure(ex.Message);
            }
        }
    }
}
