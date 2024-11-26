using System.Net.Http.Json;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PIMFazendaUrbanaAPI.DTOs;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PIMFazendaUrbanaRadzen.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;
        private readonly string _endpointUrl;

        public AuthService(HttpClient httpClient, string endpointUrl, ILocalStorageService localStorage, CustomAuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _endpointUrl = endpointUrl;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            Console.WriteLine("AuthService initialized");
        }

        public async Task<string> LoginAsync(LoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync(_endpointUrl, loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                var token = result["token"].ToString();

                var funcionarioJson = result["funcionario"].ToString();

                var funcionario = JsonConvert.DeserializeObject<FuncionarioSessionDTO>(funcionarioJson);

                await _localStorage.SetItemAsync("authToken", token);

                await _localStorage.SetItemAsync("funcionario", funcionarioJson);

                // notifica o provedor de autenticação sobre o login bem-sucedido
                _authenticationStateProvider.NotifyUserAuthentication(token);

                return null; // retorna null se o login for bem-sucedido
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                return errorResponse["message"]?.ToString();
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("funcionario");

            // notifica o provedor de autenticação sobre o logout
            _authenticationStateProvider.NotifyUserLogout();
        }

        public async Task<FuncionarioSessionDTO> GetCurrentUserAsync()
        {
            var funcionarioJson = await _localStorage.GetItemAsync<string>("funcionario");

            return funcionarioJson != null
                ? JsonConvert.DeserializeObject<FuncionarioSessionDTO>(funcionarioJson)
                : null;
        }

        public async Task<string> GetCurrentUserRole()
        {
            var funcionario = await GetCurrentUserAsync();

            return funcionario != null
                ? funcionario.Cargo
                : "Anônimo";
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await GetAuthTokenAsync();
            return token != null;
        }

        public async Task<bool> IsGerente()
        {
            var role = await GetCurrentUserRole();
            return role == "Gerente";
        }

        public async Task<string> GetAuthTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>("authToken");
        }
    }
}
