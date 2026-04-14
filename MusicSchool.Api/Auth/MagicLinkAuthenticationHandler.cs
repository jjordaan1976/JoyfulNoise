using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MusicSchool.Data.Interfaces;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MusicSchool.Api.Auth
{
    public class MagicLinkAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "MagicLink";
        public const string ClaimEntityId = "EntityId";
        public const string ClaimEntityType = "EntityType";

        private readonly IMagicLinkRepository _magicLinkRepository;

        public MagicLinkAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IMagicLinkRepository magicLinkRepository)
            : base(options, logger, encoder)
        {
            _magicLinkRepository = magicLinkRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Magic-Token", out var tokenHeader))
                return AuthenticateResult.NoResult();

            var tokenString = tokenHeader.FirstOrDefault();
            if (!Guid.TryParse(tokenString, out var token))
                return AuthenticateResult.Fail("Invalid magic link token format.");

            var link = await _magicLinkRepository.GetByTokenAsync(token);

            if (link is null || !link.IsActive)
                return AuthenticateResult.Fail("Magic link not found or has been revoked.");

            if (link.ExpiresAt.HasValue && link.ExpiresAt.Value < DateTime.UtcNow)
                return AuthenticateResult.Fail("Magic link has expired.");

            var claims = new[]
            {
                new Claim(ClaimEntityId, link.EntityID.ToString()),
                new Claim(ClaimEntityType, link.LinkType)
            };
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return AuthenticateResult.Success(ticket);
        }
    }
}
