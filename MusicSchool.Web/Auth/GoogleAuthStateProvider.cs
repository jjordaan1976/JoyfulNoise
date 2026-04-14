using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MusicSchool.Auth
{
    public class GoogleAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public GoogleAuthStateProvider(AuthService authService)
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
