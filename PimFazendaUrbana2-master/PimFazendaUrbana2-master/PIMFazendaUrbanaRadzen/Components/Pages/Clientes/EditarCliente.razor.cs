using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Clientes
{
    public partial class EditarCliente
    {
        [Parameter]
        public int ClienteId { get; set; }

        [Inject]
        public ClienteApiService<ClienteDTO> ClienteApiService { get; set; }

        [Inject]
        public CepApiService CepApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected ClienteDTO cliente;
        protected bool errorVisible;

        protected List<string> estados = new List<string>
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PE", "PI", "PR", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        protected override async Task OnInitializedAsync()
        {
            cliente = new ClienteDTO();
            cliente.Telefone = new TelefoneDTO();
            cliente.Endereco = new EnderecoDTO();

            if (cliente.Nome.IsNullOrEmpty() || cliente == null)
            {
                Console.WriteLine("Cliente é nulo");
            }
            await LoadCliente();
        }

        protected async Task LoadCliente()
        {
            try
            {
                cliente = await ClienteApiService.GetByIdAsync(ClienteId);

                if (cliente == null)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Aviso", "Cliente não encontrado. Redirecionando para a lista de clientes.", duration: 5000);
                    NavigationManager.NavigateTo("/clientes");
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Erro ao carregar cliente: {ex.Message}", duration: 5000);
                NavigationManager.NavigateTo("/clientes");
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                // Limpa e formata os campos antes de enviar
                cliente.CNPJ = FormatCNPJ(cliente.CNPJ);
                cliente.Telefone.Numero = FormatTelefone(cliente.Telefone.Numero);
                cliente.Endereco.CEP = FormatCEP(cliente.Endereco.CEP);

                Console.WriteLine($"Chamando ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);
                var response = await ClienteApiService.UpdateAsync(cliente);
                Console.WriteLine("Retornou de ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Navegando para /clientes");
                    // Redireciona para a página de clientes e exibe mensagem de sucesso
                    NavigationManager.NavigateTo("/clientes");
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Cliente atualizado com sucesso!", duration: 5000);
                }
                else
                {
                    // Usando ApiResponseHelper apenas para processar resposta de erro
                    var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao atualizar cliente: {errorMessage}", duration: 10000);
                }
            }
            catch (Exception ex)
            {
                errorVisible = true; // Exibe mensagem de erro
                Console.WriteLine($"Erro ao atualizar cliente: {ex.Message}");
            }
        }


        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de clientes
            NavigationManager.NavigateTo("/clientes");
        }


        private string FormatCNPJ(string cnpj)
        {
            // Remove caracteres não numéricos e retorna o CNPJ formatado
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        private string FormatTelefone(string telefone)
        {
            // Remove caracteres não numéricos e retorna o telefone formatado
            return new string(telefone.Where(char.IsDigit).ToArray());
        }

        private string FormatCEP(string cep)
        {
            // Remove caracteres não numéricos e retorna o CEP formatado
            return new string(cep.Where(char.IsDigit).ToArray());
        }

        protected async Task BuscarEnderecoPorCEP()
        {
            try
            {
                // Limpa o formato do CEP (remove o hífen)
                string cepFormatado = FormatCEP(cliente.Endereco.CEP);

                // Chama o CepApiService para buscar o endereço
                var endereco = await CepApiService.GetEnderecoViaCep(cepFormatado);

                if (endereco != null)
                {
                    // Preenche os campos de endereço com os dados retornados
                    cliente.Endereco.Logradouro = endereco.Logradouro;
                    cliente.Endereco.Bairro = endereco.Bairro;
                    cliente.Endereco.Cidade = endereco.Cidade;
                    cliente.Endereco.UF = endereco.UF;
                    cliente.Endereco.Complemento = endereco.Complemento ?? string.Empty; // Complemento pode ser nulo
                }
                else
                {
                    // Se o endereço não for encontrado, exibe uma mensagem de erro
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "CEP não encontrado. Verifique o número do CEP e tente novamente.", duration: 5000);
                }
            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro na consulta, exibe mensagem de erro
                NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Erro ao consultar o CEP: {ex.Message}", duration: 5000);
            }
        }

    }
}
