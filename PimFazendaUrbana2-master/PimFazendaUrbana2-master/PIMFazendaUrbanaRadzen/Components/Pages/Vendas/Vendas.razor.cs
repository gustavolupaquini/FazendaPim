using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;
using Radzen.Blazor;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Vendas
{
    public partial class Vendas
    {
        [Inject]
        public VendaApiService<PedidoVendaItemDTO> VendaApiService { get; set; } // servi�o que chama a API

        [Inject]
        public NavigationManager NavigationManager { get; set; } // servi�o de navega��o

        [Inject]
        public NotificationService NotificationService { get; set; } // servi�o de notifica��o

        [Inject]
        private ExportacaoApiService<object> exportacaoApiService { get; set; } // servi�o de exporta��o para xlsx e csv

        [Inject]
        public IJSRuntime JSRuntime { get; set; } // para chamadas JavaScript

        protected List<PedidoVendaItemDTO> vendaitens; // lista de vendaitens

        protected string errorMessage = string.Empty; // mensagem de erro

        protected string searchQuery = string.Empty; // query da barra pesquisar

        protected RadzenDataGrid<PedidoVendaItemDTO> grid0; // grid de vendaitens

        protected override async Task OnInitializedAsync()
        {
            await LoadPedidoVendaItens(); // Carrega vendas inicialmente
        }

        protected async Task LoadPedidoVendaItens()
        {
            try
            {
                var todasVendas = string.IsNullOrWhiteSpace(searchQuery)
                    ? await VendaApiService.GetAllAsync()
                    : await VendaApiService.GetVendasFiltradasAsync(searchQuery);

                vendaitens = todasVendas;
                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar vendas: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        protected async Task OnSearch(string search)
        {
            searchQuery = search;
            await LoadPedidoVendaItens(); // Atualiza a lista de vendas com base na pesquisa
        }

        protected void AddButtonClick()
        {
            // A��o ao clicar no bot�o "Adicionar"
            NavigationManager.NavigateTo("/cadastrar-venda"); // Redireciona para a p�gina de cadastro de venda
        }

        protected async Task OnExportarClick(RadzenSplitButtonItem args)
        {
            if (args == null || string.IsNullOrEmpty(args.Value.ToString()))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro", "Por favor, selecione um formato de exporta��o.", duration: 2000);
                return;
            }

            string format = args.Value.ToString(); // "xlsx" ou "csv"
            string fileName = $"Vendas_{DateTime.Now:yyyyMMdd_HHmmss}";

            try
            {
                // Verifique se h� dados
                if (vendaitens == null || !vendaitens.Any())
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "N�o h� dados para exportar.", duration: 2000);
                    return;
                }

                // Chama o servi�o para exporta��o com base no formato selecionado
                var fileBytes = await exportacaoApiService.ExportarAsync(vendaitens, format, fileName);

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