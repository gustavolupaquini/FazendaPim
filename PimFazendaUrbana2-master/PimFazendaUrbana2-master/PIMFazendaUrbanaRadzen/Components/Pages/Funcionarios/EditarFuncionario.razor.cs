using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaLib;
using PIMFazendaUrbanaRadzen.Components.Pages.Fornecedores;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Funcionarios
{
    public partial class EditarFuncionario
    {
        [Parameter]
        public int FuncionarioId { get; set; }

        [Inject]
        public FuncionarioApiService<FuncionarioDTO> FuncionarioApiService { get; set; }

        [Inject]
        public CepApiService CepApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } // Inject NavigationManager

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected FuncionarioDTO funcionario;
        protected bool errorVisible;

        protected List<string> estados = new List<string>
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PE", "PI", "PR", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        private List<string> generos = new List<string> { "M", "F", "Outro" };

        private List<string> cargos = new List<string> { "Funcionário", "Gerente" };

        protected override async Task OnInitializedAsync()
        {
            funcionario = new FuncionarioDTO();
            funcionario.Telefone = new TelefoneDTO();
            funcionario.Endereco = new EnderecoDTO();

            if (funcionario.Nome.IsNullOrEmpty() || funcionario == null)
            {
                Console.WriteLine("Fornecedor é nulo");
            }
            await LoadFuncionario();
        }

        //private string nomeUsuarioOriginal { get; set; } // alterar usuario removido

        protected async Task LoadFuncionario()
        {
            try
            {
                funcionario = await FuncionarioApiService.GetByIdAsync(FuncionarioId);

                //nomeUsuarioOriginal = funcionario.Usuario; // alterar usuario removido

                if (funcionario == null)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Aviso", "Funcionário não encontrado. Redirecionando para a lista de funcionários.", duration: 5000);
                    NavigationManager.NavigateTo("/funcionarios");
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Erro ao carregar funcionários: {ex.Message}", duration: 5000);
                NavigationManager.NavigateTo("/funcionarios");
            }
        }

        protected async Task FormSubmit()
        {
            try
            {

                // Limpa e formata os campos antes de enviar
                funcionario.CPF = FormatCPF(funcionario.CPF);
                funcionario.Telefone.Numero = FormatTelefone(funcionario.Telefone.Numero);
                funcionario.Endereco.CEP = FormatCEP(funcionario.Endereco.CEP);

                Console.WriteLine($"Chamando ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);
                var response = await FuncionarioApiService.UpdateAsync(funcionario); // Chama ApiService para atualizar o funcionario
                Console.WriteLine("Retornou de ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Navegando para /funcionarios");
                    // Redireciona para a página de funcionarios e exibe mensagem de sucesso
                    NavigationManager.NavigateTo("/funcionarios");
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Funcionario atualizado com sucesso!", duration: 5000);
                }
                else
                {
                    // Usando ApiResponseHelper apenas para processar resposta de erro
                    var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao atualizar funcionário: {errorMessage}", duration: 10000);
                }
            }
            catch (Exception ex)
            {
                errorVisible = true; // Exibe mensagem de erro
                Console.WriteLine($"Erro ao atualizar funcionario: {ex.Message}");
            }
        }

        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de funcionarios
            NavigationManager.NavigateTo("/funcionarios");
        }


        private string FormatCPF(string cpf)
        {
            // Remove caracteres não numéricos e retorna o CPF formatado
            return new string(cpf.Where(char.IsDigit).ToArray());
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
                string cepFormatado = FormatCEP(funcionario.Endereco.CEP);

                // Chama o CepApiService para buscar o endereço
                var endereco = await CepApiService.GetEnderecoViaCep(cepFormatado);

                if (endereco != null)
                {
                    // Preenche os campos de endereço com os dados retornados
                    funcionario.Endereco.Logradouro = endereco.Logradouro;
                    funcionario.Endereco.Bairro = endereco.Bairro;
                    funcionario.Endereco.Cidade = endereco.Cidade;
                    funcionario.Endereco.UF = endereco.UF;
                    funcionario.Endereco.Complemento = endereco.Complemento ?? string.Empty; // Complemento pode ser nulo
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

        /* // alterar senha removido
        private string confirmaSenha;

        private bool senhasCoincidem;
        private string mensagemErroSenha;

        private bool exibirSenha = false; // Variável para controlar a visibilidade das senhas

        private async Task AlternarVisibilidadeSenha()
        {
            exibirSenha = !exibirSenha; // Alterna o valor de exibirSenha
        }

        private async Task VerificarSenhas()
        {
            if (!VerificarSenhasCoincidem())
            {
                return;
            }
            await VerificarSenhaForte();
        }

        private bool VerificarSenhasCoincidem()
        {
            if (funcionario.Senha != confirmaSenha)
            {
                senhasCoincidem = false;
                mensagemErroSenha = "Senhas não coincidem";
                return false;
            }
            else
            {
                senhasCoincidem = true;
                mensagemErroSenha = string.Empty;
                return true;
            }
        }

        protected List<string> errorMessagesSenhaForte = new List<string>();

        protected async Task VerificarSenhaForte()
        {
            string senha = funcionario.Senha;

            if (senha.IsNullOrEmpty())
            {
                return;
            }

            // Chamar VerificarSenhaForteAsync de FuncionarioApiService
            var (isStrong, errorMessages) = await FuncionarioApiService.VerificarSenhaForteAsync(senha);

            if (isStrong)
            {
                // Limpa as mensagens de erro
                errorMessagesSenhaForte.Clear();
            }
            else
            {
                // Define a lista de mensagens de erro
                errorMessagesSenhaForte = errorMessages;
            }

            // Atualiza a interface (Blazor)
            StateHasChanged();
        }
        */

        /* // alterar usuario removido
        private bool usuarioDisponivel;
        private string mensagemErroUsuarioIndisponivel;
        protected async Task VerificarUsuarioDisponivel()
        {
            if (funcionario.Usuario == nomeUsuarioOriginal)
            {
                usuarioDisponivel = true;
                mensagemErroUsuarioIndisponivel = string.Empty;
                return;
            }

            if (funcionario.Usuario.IsNullOrEmpty())
            {
                usuarioDisponivel = true;
                mensagemErroUsuarioIndisponivel = string.Empty;
                return;
            }

            // Chamar VerificarUsuarioDisponivelAsync de FuncionarioApiService
            var isAvailable = await FuncionarioApiService.VerificarUsuarioDisponivelAsync(funcionario.Usuario);

            if (isAvailable == false)
            {
                if (funcionario.Usuario.IsNullOrEmpty())
                {
                    usuarioDisponivel = true;
                    mensagemErroUsuarioIndisponivel = string.Empty;
                }
                else
                {
                    usuarioDisponivel = false;
                    mensagemErroUsuarioIndisponivel = "Usuário indisponível";
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "Nome de usuário já cadastrado. Por favor, escolha outro nome de usuário.", duration: 5000);
                }
            }
            else
            {
                usuarioDisponivel = true;
                mensagemErroUsuarioIndisponivel = string.Empty;
                NotificationService.Notify(NotificationSeverity.Success, "Usuário disponivel", "Nome de usuário disponível", duration: 2000);
            }
        }
        */

    }
}
