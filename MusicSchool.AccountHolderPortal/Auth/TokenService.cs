namespace MusicSchool.AccountHolderPortal.Auth
{
    public class TokenService
    {
        public string? Token { get; private set; }

        public void SetToken(string token) => Token = token;
        public void ClearToken() => Token = null;
    }
}
