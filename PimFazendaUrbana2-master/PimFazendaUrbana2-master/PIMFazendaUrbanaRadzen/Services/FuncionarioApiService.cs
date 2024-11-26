using Newtonsoft.Json;

namespace PIMFazendaUrbanaRadzen.Services
{
    public class FuncionarioApiService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl;

        public FuncionarioApiService(HttpClient httpClient, string endpointUrl)
        {
            _httpClient = httpClient;
            _endpointUrl = endpointUrl;
        }

        public async Task<List<T>> GetFuncionariosFiltradosAsync(string search)
        {
            try
            {
                Console.WriteLine($"Chamando API em: {_endpointUrl}/filtrados?search={Uri.EscapeDataString(search)}");
                return await _httpClient.GetFromJsonAsync<List<T>>($"{_endpointUrl}/filtrados?search={Uri.EscapeDataString(search)}");
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Erro de requisição: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao chamar API: {ex.Message}");
                throw;
            }
        }

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                Console.WriteLine($"Chamando API em: {_endpointUrl}/ativos");
                return await _httpClient.GetFromJsonAsync<List<T>>($"{_endpointUrl}/ativos");
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Erro de requisição: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao chamar API: {ex.Message}");
                throw;
            }
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_endpointUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default; // Retorna null para 404
            }
            else
            {
                // Lança exceção para outros erros
                throw new Exception($"Erro ao buscar funcionário: {response.ReasonPhrase}");
            }
        }

        public async Task<HttpResponseMessage> CreateAsync(T entity)
        {
            return await _httpClient.PostAsJsonAsync($"{_endpointUrl}/cadastrar", entity);
        }

        public async Task<HttpResponseMessage> UpdateAsync(T entity)
        {
            return await _httpClient.PutAsJsonAsync($"{_endpointUrl}/alterar", entity);
        }

        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{_endpointUrl}/excluir/{id}");
        }

        public async Task<(bool IsSuccess, List<string> ErrorMessages)> VerificarSenhaForteAsync(string senha)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpointUrl}/senha-forte/{Uri.EscapeDataString(senha)}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var resultObj = JsonConvert.DeserializeObject<dynamic>(result);
                    bool isStrong = resultObj?.forte ?? false; // Extrai o valor do campo "forte"
                    return (IsSuccess: isStrong, ErrorMessages: new List<string>());
                }
                else
                {
                    // Log completo do conteúdo da resposta
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Resposta de erro: {errorContent}");

                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        Console.WriteLine("Bad Request recebida");

                        try
                        {
                            // Tenta desserializar o JSON de erro usando o Newtonsoft.Json
                            var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorContent);

                            // Concatena as mensagens de erro em uma lista
                            var errorMessages = errorResponse?.Errors.Select(e => e.Mensagem).ToList();
                            return (IsSuccess: false, ErrorMessages: errorMessages);
                        }
                        catch (JsonException jsonEx)
                        {
                            // Log caso a desserialização falhe
                            Console.WriteLine($"Erro ao desserializar JSON de erro: {jsonEx.Message}");
                            return (IsSuccess: false, ErrorMessages: new List<string> { "Erro ao interpretar a resposta do servidor." });
                        }
                    }

                    return (IsSuccess: false, ErrorMessages: new List<string> { "Erro inesperado ao verificar senha." });
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Erro de requisição: {httpEx.Message}");
                return (IsSuccess: false, ErrorMessages: new List<string> { "Erro ao conectar com o servidor." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao chamar API: {ex.Message}");
                return (IsSuccess: false, ErrorMessages: new List<string> { "Erro inesperado." });
            }
        }

        // método para verificar usuario disponivel
        public async Task<bool> VerificarUsuarioDisponivelAsync(string usuario)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_endpointUrl}/usuario-disponivel/{Uri.EscapeDataString(usuario)}");
                var responseContent = await response.Content.ReadAsStringAsync();
                var resultObj = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return resultObj?.disponivel ?? false;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Erro de requisição: {httpEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao chamar API: {ex.Message}");
                throw;
            }
        }

    }
}
