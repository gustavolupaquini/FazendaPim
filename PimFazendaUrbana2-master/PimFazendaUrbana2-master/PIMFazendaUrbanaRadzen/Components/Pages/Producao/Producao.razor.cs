using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;
using Radzen.Blazor;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Producao
{
    public partial class Producao
    {
        [Inject]
        public ProducaoApiService<ProducaoDTO> ProducaoApiService { get; set; } // serviço que chama a API

        [Inject]
        public NavigationManager NavigationManager { get; set; } // serviço de navegação

        [Inject]
        public NotificationService NotificationService { get; set; } // serviço de notificação

        [Inject]
        private ExportacaoApiService<object> exportacaoApiService { get; set; } // serviço de exportação para xlsx e csv

        [Inject]
        public IJSRuntime JSRuntime { get; set; } // para chamadas JavaScript

        protected List<ProducaoDTO> producoes; // lista de produções

        protected string errorMessage = string.Empty; // mensagem de erro

        protected string searchQuery = string.Empty; // query da barra pesquisar

        protected RadzenDataGrid<ProducaoDTO> grid0; // grid de produções

        protected override async Task OnInitializedAsync()
        {
            await LoadProducoes(); // Carrega produções inicialmente
        }

        protected async Task LoadProducoes()
        {
            try
            {
                var todasProducoes = string.IsNullOrWhiteSpace(searchQuery)
                    ? await ProducaoApiService.GetAllAsync()
                    : await ProducaoApiService.GetProducoesFiltradasAsync(searchQuery);

                producoes = todasProducoes;
                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar produções: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }


        protected async Task OnSearch(string search)
        {
            searchQuery = search;
            await LoadProducoes(); // Atualiza a lista de producoes com base na pesquisa
        }

        protected void AddButtonClick()
        {
            // Ação ao clicar no botão "Adicionar"
            NavigationManager.NavigateTo("/cadastrar-producao"); // Redireciona para a página de cadastro de produção
        }

        protected void OnRowSelect(ProducaoDTO producao)
        {
            // Ação ao selecionar uma linha
        }

        protected async Task FinalizarProducao(ProducaoDTO producao)
        {
            try
            {
                await ProducaoApiService.FinishAsync(producao.Id);
                await LoadProducoes(); // Atualiza a lista após finalizar
                NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Produção finalizada com sucesso!", duration: 3000);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro ao finalizar", ex.Message, duration: 5000);
            }
        }

        [Inject]
        protected DialogService DialogService { get; set; }

        private async Task ConfirmarFinalizacao(ProducaoDTO producao)
        {
            if (producao.StatusFinalizado == false)
            {
                bool? confirm = await DialogService.Confirm($"Tem certeza que deseja finalizar a produção de {producao.Cultivo.Variedade} de {producao.Data.ToShortDateString()}?",
                                                             "Confirmação de Finalização",
                                                             new ConfirmOptions { OkButtonText = "Finalizar", CancelButtonText = "Cancelar" });

                if (confirm == true)
                {
                    await FinalizarProducao(producao);
                }
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Info, "Produção já finalizada", duration: 2000);
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
            string fileName = $"Producoes_{DateTime.Now:yyyyMMdd_HHmmss}";

            try
            {
                // Verifique se há dados
                if (producoes == null || !producoes.Any())
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "Não há dados para exportar.", duration: 2000);
                    return;
                }

                // Chama o serviço para exportação com base no formato selecionado
                var fileBytes = await exportacaoApiService.ExportarAsync(producoes, format, fileName);

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
