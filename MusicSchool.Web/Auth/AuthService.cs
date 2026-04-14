using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace MusicSchool.Auth
{
    public class AuthService
    {
        private readonly IJSRuntime _js;
        private string? _token;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public event Action? AuthStateChanged;

        public ClaimsPrincipal CurrentUser => _currentUser;
        public string? Token => _token;
        public bool IsAuthenticated => _currentUser.Identity?.IsAuthenticated == true;

        public AuthService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitializeAsync()
        {
            var token = await _js.InvokeAsync<string?>("sessionStorage.getItem", "google_token");
            if (!string.IsNullOrEmpty(token) && !IsTokenExpired(token))
                SetToken(token);
        }

        [JSInvokable]
        public async Task HandleCredential(string credential)
        {
            SetToken(credential);
            await _js.InvokeVoidAsync("sessionStorage.setItem", "google_token", credential);
            AuthStateChanged?.Invoke();
        }

        public async Task SignOutAsync()
        {
            _token = null;
            _currentUser = new(new ClaimsIdentity());
            await _js.InvokeVoidAsync("sessionStorage.removeItem", "google_token");
            await _js.InvokeVoidAsync("googleAuth.signOut");
            AuthStateChanged?.Invoke();
        }

        private void SetToken(string token)
        {
            _token = token;
            _currentUser = ParseJwt(token);
        }

        private static bool IsTokenExpired(string token)
        {
            try
            {
                var payload = DecodePayload(token);
                if (payload.TryGetProperty("exp", out var exp))
                    return DateTimeOffset.FromUnixTimeSeconds(exp.GetInt64()) <= DateTimeOffset.UtcNow;
                return true;
            }
            catch { return true; }
        }

        private static ClaimsPrincipal ParseJwt(string token)
        {
            try
            {
                var payload = DecodePayload(token);
                var claims = new List<Claim>();

                foreach (var prop in payload.EnumerateObject())
                    claims.Add(new Claim(prop.Name, prop.Value.ToString()));

                if (payload.TryGetProperty("email", out var email))
                    claims.Add(new Claim(ClaimTypes.Email, email.GetString()!));
                if (payload.TryGetProperty("name", out var name))
                    claims.Add(new Claim(ClaimTypes.Name, name.GetString()!));

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "Google"));
            }
            catch { return new ClaimsPrincipal(new ClaimsIdentity()); }
        }

        private static JsonElement DecodePayload(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3) throw new ArgumentException("Invalid JWT");

            var padded = parts[1].Replace('-', '+').Replace('_', '/');
            padded = (padded.Length % 4) switch
            {
                2 => padded + "==",
                3 => padded + "=",
                _ => padded
            };

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            return JsonDocument.Parse(json).RootElement;
        }
    }
}
