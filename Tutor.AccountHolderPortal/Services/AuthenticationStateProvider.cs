using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Tutor.AccountHolderPortal.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly AuthenticationState Anonymous =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        private readonly IAuthService _authService;

        public JwtAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Anonymous;
            }

            var claims = ParseClaimsFromJwt(token);
            if (claims == null)
            {
                return Anonymous;
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyAuthenticationChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private static IEnumerable<Claim>? ParseClaimsFromJwt(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2) return null;

            try
            {
                var payload = parts[1];
                var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
                    .Replace('-', '+').Replace('_', '/');
                var bytes = Convert.FromBase64String(padded);
                var json = Encoding.UTF8.GetString(bytes);
                using var doc = JsonDocument.Parse(json);

                var claims = new List<Claim>();
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in prop.Value.EnumerateArray())
                            claims.Add(new Claim(prop.Name, item.ToString()));
                    }
                    else
                    {
                        claims.Add(new Claim(prop.Name, prop.Value.ToString()));
                    }
                }
                return claims;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
