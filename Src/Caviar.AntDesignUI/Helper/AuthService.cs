using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace Caviar.AntDesignUI.Helper
{
    public class AuthService
    {
        private readonly HttpHelper _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpHelper httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }


        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(Config.TokenName);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.HttpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

}
