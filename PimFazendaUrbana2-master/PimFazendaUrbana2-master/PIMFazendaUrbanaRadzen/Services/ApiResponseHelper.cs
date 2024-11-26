using Newtonsoft.Json;

namespace PIMFazendaUrbanaRadzen.Services
{
    public static class ApiResponseHelper
    {
        // Método para processar a resposta de erro
        public static async Task<string> HandleErrorResponseAsync(HttpResponseMessage response)
        {
            try
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Resposta de erro: {errorContent}");

                // Verifica o erro específico (ex: BadRequest)
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(errorContent);
                    var errorMessage = string.Join("; ", errorResponse?.Errors.Select(e => e.Mensagem));
                    return errorMessage;
                }

                // Caso de erro genérico
                return "Erro inesperado ao processar a requisição.";
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Erro ao desserializar JSON de erro: {jsonEx.Message}");
                return "Erro ao interpretar a resposta do servidor.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a resposta: {ex.Message}");
                return "Erro inesperado.";
            }
        }
    }
}
