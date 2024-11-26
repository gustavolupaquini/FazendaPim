using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Fornecedores
{
    public partial class EditarFornecedor
    {
        [Parameter]
        public int FornecedorId { get; set; }

        [Inject]
        public FornecedorApiService<FornecedorDTO> FornecedorApiService { get; set; }

        [Inject]
        public CepApiService CepApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected FornecedorDTO fornecedor;
        protected bool errorVisible;

        protected List<string> estados = new List<string>
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PE", "PI", "PR", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        protected override async Task OnInitializedAsync()
        {
            fornecedor = new FornecedorDTO();
            fornecedor.Telefone = new TelefoneDTO();
            fornecedor.Endereco = new EnderecoDTO();

            if (fornecedor.Nome.IsNullOrEmpty() || fornecedor == null)
            {
                Console.WriteLine("Fornecedor é nulo");
            }
            await LoadFornecedor();
        }

        protected async Task LoadFornecedor()
        {
            try
            {
                fornecedor = await FornecedorApiService.GetByIdAsync(FornecedorId);

                if (fornecedor == null)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Aviso", "Fornecedor não encontrado. Redirecionando para a lista de fornecedores.", duration: 5000);
                    NavigationManager.NavigateTo("/fornecedores");
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Erro ao carregar fornecedor: {ex.Message}", duration: 5000);
                NavigationManager.NavigateTo("/fornecedores");
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                // Limpa e formata os campos antes de enviar
                fornecedor.CNPJ = FormatCNPJ(fornecedor.CNPJ);
                fornecedor.Telefone.Numero = FormatTelefone(fornecedor.Telefone.Numero);
                fornecedor.Endereco.CEP = FormatCEP(fornecedor.Endereco.CEP);

                Console.WriteLine($"Chamando ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);
                var response = await FornecedorApiService.UpdateAsync(fornecedor);
                Console.WriteLine("Retornou de ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Navegando para /fornecedores");
                    // Redireciona para a página de fornecedores e exibe mensagem de sucesso
                    NavigationManager.NavigateTo("/fornecedores");
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Fornecedor atualizado com sucesso!", duration: 5000);
                }
                else
                {
                    // Usando ApiResponseHelper apenas para processar resposta de erro
                    var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao atualizar fornecedor: {errorMessage}", duration: 10000);
                }
            }
            catch (Exception ex)
            {
                errorVisible = true; // Exibe mensagem de erro
                Console.WriteLine($"Erro ao atualizar fornecedor: {ex.Message}");
            }
        }

        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de fornecedores
            NavigationManager.NavigateTo("/fornecedores");
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
                string cepFormatado = FormatCEP(fornecedor.Endereco.CEP);

                // Chama o CepApiService para buscar o endereço
                var endereco = await CepApiService.GetEnderecoViaCep(cepFormatado);

                if (endereco != null)
                {
                    // Preenche os campos de endereço com os dados retornados
                    fornecedor.Endereco.Logradouro = endereco.Logradouro;
                    fornecedor.Endereco.Bairro = endereco.Bairro;
                    fornecedor.Endereco.Cidade = endereco.Cidade;
                    fornecedor.Endereco.UF = endereco.UF;
                    fornecedor.Endereco.Complemento = endereco.Complemento ?? string.Empty; // Complemento pode ser nulo
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
