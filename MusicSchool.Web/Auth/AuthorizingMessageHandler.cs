using System.Net.Http.Headers;

namespace MusicSchool.Auth
{
    public class AuthorizingMessageHandler : DelegatingHandler
    {
        private readonly AuthService _authService;

        public AuthorizingMessageHandler(AuthService authService)
        {
            _authService = authService;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_authService.Token))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _authService.Token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
