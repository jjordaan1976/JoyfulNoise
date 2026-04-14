namespace MusicSchool.AccountHolderPortal.Auth
{
    public class MagicLinkHandler : DelegatingHandler
    {
        private readonly TokenService _tokenService;

        public MagicLinkHandler(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(_tokenService.Token))
                request.Headers.TryAddWithoutValidation("X-Magic-Token", _tokenService.Token);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
