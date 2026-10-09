using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Tutor.Data.Interfaces;

namespace Tutor.Api.Auth
{
    public static class BypassAuthenticationDefaults
    {
        public const string Scheme = "Bypass";
    }

    public class BypassAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>The user every request is authenticated as while Auth:Enabled is false.</summary>
        public string BypassEmail { get; set; } = string.Empty;
    }

    /// <summary>
    /// Used instead of JWT bearer while Auth:Enabled is false. Every request is authenticated as the
    /// configured user, with the same claims a real login would have issued, so controllers and
    /// ICurrentUserService behave identically with authentication on or off.
    /// </summary>
    public class BypassAuthenticationHandler : AuthenticationHandler<BypassAuthenticationOptions>
    {
        private readonly IAuthRepository _authRepository;

        public BypassAuthenticationHandler(
            IOptionsMonitor<BypassAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IAuthRepository authRepository)
            : base(options, logger, encoder)
        {
            _authRepository = authRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var claims = await _authRepository.GetUserClaimsAsync(Options.BypassEmail);

                var identity = new ClaimsIdentity(Scheme.Name);
                identity.AddClaim(new Claim(ClaimTypes.Email, Options.BypassEmail));
                foreach (var claim in claims)
                    identity.AddClaim(new Claim(claim.Key, claim.Value));

                return AuthenticateResult.Success(
                    new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Auth bypass could not resolve a user for {Email}", Options.BypassEmail);
                return AuthenticateResult.Fail(ex.Message);
            }
        }
    }
}
