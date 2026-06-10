using Microsoft.JSInterop;

namespace Tutor.Web.Services
{
    public interface IAuthService
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task ClearTokenAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IJSRuntime _jsRuntime;

        public AuthService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "auth_token");
            }
            catch
            {
                return null;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "auth_token", token);
        }

        public async Task ClearTokenAsync()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "auth_token");
        }
    }
}
