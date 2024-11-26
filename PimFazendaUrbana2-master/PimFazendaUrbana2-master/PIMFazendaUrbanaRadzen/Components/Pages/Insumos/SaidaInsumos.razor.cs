using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;
using Radzen.Blazor;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Insumos
{
    public partial class SaidaInsumos
    {
        [Inject]
        public InsumoApiService<SaidaInsumoDTO> InsumoApiService { get; set; } // serviço que chama a API

        [Inject]
        public NavigationManager NavigationManager { get; set; } // serviço de navegação

        [Inject]
        public NotificationService NotificationService { get; set; } // serviço de notificação

        [Inject]
        private ExportacaoApiService<object> exportacaoApiService { get; set; } // serviço de exportação para xlsx e csv

        [Inject]
        public IJSRuntime JSRuntime { get; set; } // para chamadas JavaScript

        protected List<InsumoDTO> insumos; // lista de insumos

        protected List<SaidaInsumoDTO> saidaInsumos; // lista de insumos

        protected string errorMessage = string.Empty; // mensagem de erro

        protected string searchQuery = string.Empty; // query da barra pesquisar

        protected RadzenDataGrid<SaidaInsumoDTO> grid0; // grid de insumos

        protected string enderecoFiltro = string.Empty; // filtro para todos as propriedades de endereço, que o usuário pode digitar

        protected override async Task OnInitializedAsync()
        {
            await LoadSaidaInsumos(); // Carrega insumos inicialmente
        }

        protected async Task OnEnderecoFilterChanged()
        {
            await LoadSaidaInsumos(); // Recarrega os dados aplicando o filtro de endereço
        }

        protected async Task OnEnderecoFilterClear()
        {
            enderecoFiltro = string.Empty; // Limpa o filtro
            await LoadSaidaInsumos();          // Recarrega os dados sem filtro
        }

        protected async Task LoadSaidaInsumos()
        {
            try
            {
                var todosInsumos = string.IsNullOrWhiteSpace(searchQuery)
                    ? await InsumoApiService.GetDecreaseAsync()
                    : await InsumoApiService.GetSaidaInsumosFiltradosAsync(searchQuery);

                saidaInsumos = todosInsumos;
                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar insumos: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }


        protected async Task OnSearch(string search)
        {
            searchQuery = search;
            await LoadSaidaInsumos(); // Atualiza a lista de insumos com base na pesquisa
        }

        protected void AddButtonClick()
        {
            // Ação ao clicar no botão "Adicionar"
            NavigationManager.NavigateTo("/cadastrar-insumo"); // Redireciona para a página de cadastro de insumos
        }
        protected void ReturnButtonClick()
        {
            // Ação ao clicar no botão "Adicionar"
            NavigationManager.NavigateTo("/insumos"); // Redireciona para a página de cadastro de insumos
        }

        protected void OnRowSelect(InsumoDTO insumo)
        {
            // Ação ao selecionar uma linha (insumos)
        }

        protected void BaixarInsumo(InsumoDTO insumo)
        {
            if (insumo?.Id != null)
            {
                Console.WriteLine($"Navegando para /baixar-insumo/{insumo.Id}");
                NavigationManager.NavigateTo($"/baixar-insumo/{insumo.Id}");
            }
            else
            {
                Console.WriteLine("Erro: ID do insumo é nulo.");
            }
        }

        protected async Task InativarInsumo(InsumoDTO insumo)
        {
            
            try
            {
                await InsumoApiService.DeleteAsync(insumo.Id);
                await LoadSaidaInsumos(); // Atualiza a lista após exclusão
                NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Insumo inativado com sucesso!", duration: 3000);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro ao excluir", ex.Message, duration: 5000);
            }
            
        }

        [Inject]
        protected DialogService DialogService { get; set; }

        private async Task ConfirmarExclusao(InsumoDTO insumo)
        {
            if (insumo.Qtd > 0)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Não é possível excluir o item, pois ainda há estoque.", duration: 5000);
                return;
            }

            bool? confirm = await DialogService.Confirm($"Tem certeza que deseja inativar {insumo.Nome}?",
                                                         "Confirmação de Exclusão",
                                                         new ConfirmOptions { OkButtonText = "Inativar", CancelButtonText = "Cancelar" });

            if (confirm == true)
            {
                await InativarInsumo(insumo);
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
            string fileName = $"SaidaInsumos{DateTime.Now:yyyyMMdd_HHmmss}";

            try
            {
                // Verifique se há dados
                if (saidaInsumos == null || !saidaInsumos.Any())
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "Não há dados para exportar.", duration: 2000);
                    return;
                }

                // Chama o serviço para exportação com base no formato selecionado
                var fileBytes = await exportacaoApiService.ExportarAsync(saidaInsumos, format, fileName);

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
