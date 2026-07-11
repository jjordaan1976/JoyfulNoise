using System.Security.Claims;
using Tutor.Data.Interfaces;

namespace Tutor.Api.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public string? Email => Principal?.FindFirst(ClaimTypes.Email)?.Value;
        public string? DisplayName => Principal?.FindFirst("name")?.Value;
        public int? TeacherId => GetIntClaim("teacherId");
        public int? StudentId => GetIntClaim("studentId");
        public int? AccountHolderId => GetIntClaim("accountHolderId");
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

        public int RequireTeacherId() =>
            TeacherId ?? throw new InvalidOperationException("Teacher not found in token claims.");

        public int RequireStudentId() =>
            StudentId ?? throw new InvalidOperationException("Student not found in token claims.");

        public int RequireAccountHolderId() =>
            AccountHolderId ?? throw new InvalidOperationException("Account holder not found in token claims.");

        private int? GetIntClaim(string claimType)
        {
            var value = Principal?.FindFirst(claimType)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
