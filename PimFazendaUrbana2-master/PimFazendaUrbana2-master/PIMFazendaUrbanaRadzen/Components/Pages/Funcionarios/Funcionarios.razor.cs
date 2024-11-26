using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaLib;
using PIMFazendaUrbanaRadzen.Components.Pages.Clientes;
using PIMFazendaUrbanaRadzen.Components.Pages.Fornecedores;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;
using Radzen.Blazor;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Funcionarios
{
    public partial class Funcionarios
    {
        [Inject]
        public FuncionarioApiService<FuncionarioDTO> FuncionarioApiService { get; set; } // serviço que chama a API

        [Inject]
        public NavigationManager NavigationManager { get; set; } // serviço de navegação

        [Inject]
        public NotificationService NotificationService { get; set; } // serviço de notificação

        [Inject]
        private ExportacaoApiService<object> exportacaoApiService { get; set; } // serviço de exportação para xlsx e csv

        [Inject]
        public IJSRuntime JSRuntime { get; set; } // para chamadas JavaScript

        protected List<FuncionarioDTO> funcionarios; // lista de funcionarios

        protected string errorMessage = string.Empty; // mensagem de erro

        protected string searchQuery = string.Empty; // query da barra pesquisar

        protected RadzenDataGrid<FuncionarioDTO> grid0; // grid de funcionarios

        protected string enderecoFiltro = string.Empty; // filtro para todos as propriedades de endereço, que o usuário pode digitar

        protected override async Task OnInitializedAsync()
        {
            await LoadFuncionarios(); // Carrega funcionarios inicialmente
        }

        protected async Task OnEnderecoFilterChanged()
        {
            await LoadFuncionarios(); // Recarrega os dados aplicando o filtro de endereço
        }

        protected async Task OnEnderecoFilterClear()
        {
            enderecoFiltro = string.Empty; // Limpa o filtro
            await LoadFuncionarios();          // Recarrega os dados sem filtro
        }

        protected async Task LoadFuncionarios()
        {
            try
            {
                var todosFuncionarios = string.IsNullOrWhiteSpace(searchQuery)
                    ? await FuncionarioApiService.GetAllAsync()
                    : await FuncionarioApiService.GetFuncionariosFiltradosAsync(searchQuery);

                // Filtro de endereço personalizado
                if (!string.IsNullOrWhiteSpace(enderecoFiltro))
                {
                    todosFuncionarios = todosFuncionarios.Where(funcionario =>
                        funcionario.Endereco != null && (
                            (funcionario.Endereco.Logradouro?.Contains(enderecoFiltro, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (funcionario.Endereco.Numero?.Contains(enderecoFiltro, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (funcionario.Endereco.Bairro?.Contains(enderecoFiltro, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (funcionario.Endereco.Cidade?.Contains(enderecoFiltro, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (funcionario.Endereco.UF?.Contains(enderecoFiltro, StringComparison.OrdinalIgnoreCase) ?? false)
                        )
                    ).ToList();
                }

                funcionarios = todosFuncionarios;
                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar funcionários: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        protected async Task OnSearch(string search)
        {
            searchQuery = search;
            await LoadFuncionarios(); // Atualiza a lista de funcionarios com base na pesquisa
        }

        protected void AddButtonClick()
        {
            // Ação ao clicar no botão "Adicionar"
            NavigationManager.NavigateTo("/cadastrar-funcionario"); // Redireciona para a página de cadastro de funcionario
        }

        protected void OnRowSelect(FuncionarioDTO funcionario)
        {
            // Ação ao selecionar uma linha (funcionario)
        }

        protected void EditarFuncionario(FuncionarioDTO funcionario)
        {
            if (funcionario?.Id != null)
            {
                NavigationManager.NavigateTo($"/editar-funcionario/{funcionario.Id}");
            }
            else
            {
                Console.WriteLine("Erro: ID do funcionário é nulo.");
            }
        }

        protected async Task ExcluirFuncionario(FuncionarioDTO funcionario)
        {
            try
            {
                await FuncionarioApiService.DeleteAsync(funcionario.Id);
                await LoadFuncionarios(); // Atualiza a lista após exclusão
                NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Funcionário excluído com sucesso!", duration: 3000);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro ao excluir", ex.Message, duration: 5000);
            }

        }

        [Inject]
        protected DialogService DialogService { get; set; }

        private async Task ConfirmarExclusao(FuncionarioDTO funcionario)
        {
            bool? confirm = await DialogService.Confirm($"Tem certeza que deseja excluir {funcionario.Nome}?",
                                                         "Confirmação de Exclusão",
                                                         new ConfirmOptions { OkButtonText = "Excluir", CancelButtonText = "Cancelar" });

            if (confirm == true)
            {
                await ExcluirFuncionario(funcionario);
            }
        }

        protected string FormatarTelefone(string ddd, string numero)
        {
            if (numero.Length == 8)
            {
                return $"({ddd}) {numero.Substring(0, 4)}-{numero.Substring(4)}";
            }
            else if (numero.Length == 9)
            {
                return $"({ddd}) {numero.Substring(0, 5)}-{numero.Substring(5)}";
            }
            else
            {
                return "Telefone inválido";
            }
        }

        protected string FormatarCPF(string cpf)
        {
            if (cpf.Length == 11)
            {
                return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9)}";
            }
            else
            {
                return "CPF inválido";
            }
        }

        protected string FormatarCEP(string cep)
        {
            if (cep.Length == 8)
            {
                return $"{cep.Substring(0, 5)}-{cep.Substring(5)}";
            }
            else
            {
                return "CEP inválido";
            }
        }

        protected async Task OnExportarClick(RadzenSplitButtonItem args)
        {
            if (args == null || string.IsNullOrEmpty(args.Value.ToString()))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro", "Por favor, selecione um formato de exportação.", duration: 2000);
                return;
            }

            string format = args.Value.ToString(); // "xlsx" ou "csv"
            string fileName = $"Funcionarios_{DateTime.Now:yyyyMMdd_HHmmss}";

            try
            {
                // Verifique se há dados
                if (funcionarios == null || !funcionarios.Any())
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "Não há dados para exportar.", duration: 2000);
                    return;
                }

                // Chama o serviço para exportação com base no formato selecionado
                var fileBytes = await exportacaoApiService.ExportarAsync(funcionarios, format, fileName);

                if (fileBytes != null)
                {
                    // Gera o download no navegador
                    await JSRuntime.InvokeVoidAsync("downloadFile", fileName + "." + format, Convert.ToBase64String(fileBytes));
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro ao exportar", "Nenhum arquivo foi gerado.", duration: 2000);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro ao exportar", ex.Message, duration: 5000);
            }
        }

    }
}
