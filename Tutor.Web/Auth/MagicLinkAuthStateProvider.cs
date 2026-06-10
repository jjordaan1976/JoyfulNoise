using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Tutor.Auth
{
    public class MagicLinkAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public MagicLinkAuthStateProvider(AuthService authService)
        {
            _authService = authService;
            _authService.AuthStateChanged += OnAuthStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(_authService.CurrentUser));

        private void OnAuthStateChanged()
            => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
