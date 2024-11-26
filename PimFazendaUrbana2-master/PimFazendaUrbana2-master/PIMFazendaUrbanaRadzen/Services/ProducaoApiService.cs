namespace PIMFazendaUrbanaRadzen.Services
{
    public class ProducaoApiService<T>
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl;

        public ProducaoApiService(HttpClient httpClient, string endpointUrl)
        {
            _httpClient = httpClient;
            _endpointUrl = endpointUrl;
        }

        public async Task<List<T>> GetProducoesFiltradasAsync(string search)
        {
            try
            {
                Console.WriteLine($"Chamando API em: {_endpointUrl}/filtradas?search={Uri.EscapeDataString(search)}");
                return await _httpClient.GetFromJsonAsync<List<T>>($"{_endpointUrl}/filtradas?search={Uri.EscapeDataString(search)}");
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
                Console.WriteLine($"Chamando API em: {_endpointUrl}/listar");
                return await _httpClient.GetFromJsonAsync<List<T>>($"{_endpointUrl}/listar");
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
            return await _httpClient.GetFromJsonAsync<T>($"{_endpointUrl}/{id}");
        }

        public async Task<HttpResponseMessage> CreateAsync(T entity)
        {
            return await _httpClient.PostAsJsonAsync($"{_endpointUrl}/cadastrar", entity);
        }      

        public async Task<HttpResponseMessage> FinishAsync(int id)
        {
            return await _httpClient.PatchAsJsonAsync($"{_endpointUrl}/finalizar/{id}", true);
        }


    }
}
