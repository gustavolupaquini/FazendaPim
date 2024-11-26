using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;
using Radzen.Blazor;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Cultivos
{
    public partial class Cultivos
    {
        [Inject]
        public CultivoApiService<CultivoDTO> CultivoApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } // Inject NavigationManager

        [Inject]
        public NotificationService NotificationService { get; set; }

        [Inject]
        private ExportacaoApiService<object> exportacaoApiService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected List<CultivoDTO> cultivos;
        protected string errorMessage = string.Empty;
        protected string searchQuery = string.Empty;

        protected CultivoDTO cultivoCadastrarOuEditar = new CultivoDTO();
        protected bool errorVisible;

        protected RadzenDataGrid<CultivoDTO> grid0;

        protected bool isModoEditar = false;

        protected string textoCadastrarOuEditar = "Cadastrar Cultivo";

        protected List<string> categorias = new List<string>
        {
            "Verdura",
            "Legume",
            "Fruta",
            "Outro"
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadCultivos(); // Carrega cultivos inicialmente
        }

        protected async Task LoadCultivos()
        {
            try
            {
                cultivos = string.IsNullOrWhiteSpace(searchQuery)
                    ? await CultivoApiService.GetAllAsync() // Carrega todos os cultivos
                    : await CultivoApiService.GetCultivosFiltradosAsync(searchQuery); // Busca cultivos filtrados

                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar cultivos: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        protected async Task OnSearch(string search)
        {
            searchQuery = search;
            await LoadCultivos(); // Atualiza a lista de cultivos com base na pesquisa
        }

        protected async Task FormSubmit()
        {
            if (isModoEditar == false) // salvando um cadastro
            {
                try
                {
                    // Limpa e formata os campos antes de enviar
                    // acho que não precisa formatar nada

                    cultivoCadastrarOuEditar.StatusAtivo = true; // Define StatusAtivo como true por padrão

                    Console.WriteLine($"Chamando ApiService: CreateAsync" + " hora atual: " + DateTime.Now);
                    var response = await CultivoApiService.CreateAsync(cultivoCadastrarOuEditar); // Chama ApiService para criar o cultivo
                    Console.WriteLine("Retornou de ApiService: Create Async" + " hora atual: " + DateTime.Now);

                    if (response.IsSuccessStatusCode)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Cultivo cadastrado com sucesso!", duration: 5000);
                        // atualizar datagrid
                        await LoadCultivos();
                        cultivoCadastrarOuEditar = new CultivoDTO();
                    }
                    else
                    {
                        // Usando ApiResponseHelper apenas para processar resposta de erro
                        var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                        NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao cadastrar cultivo: {errorMessage}", duration: 10000);
                    }
                }
                catch (Exception ex)
                {
                    errorVisible = true; // Exibe mensagem de erro
                    Console.WriteLine($"Erro ao cadastrar cultivo: {ex.Message}");
                }
            }
            else if (isModoEditar == true) // salvando uma edição
            {
                try
                {
                    Console.WriteLine($"Chamando ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);
                    var response = await CultivoApiService.UpdateAsync(cultivoCadastrarOuEditar); // Chama ApiService para atualizar o cultivo
                    Console.WriteLine("Retornou de ApiService: Update Async" + " hora atual: " + DateTime.Now);

                    if (response.IsSuccessStatusCode)
                    {
                        NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Cultivo atualizado com sucesso!", duration: 5000);

                        AlternarModoEditar();

                        await LoadCultivos(); // atualizar datagrid
                    }
                    else
                    {
                        // Usando ApiResponseHelper apenas para processar resposta de erro
                        var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                        NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao atualizar cultivo: {errorMessage}", duration: 10000);
                    }
                }
                catch (Exception ex)
                {
                    errorVisible = true; // Exibe mensagem de erro
                    Console.WriteLine($"Erro ao cadastrar cultivo: {ex.Message}");
                }   
            }

        }

        protected void OnRowSelect(CultivoDTO cultivo)
        {
            // Ação ao selecionar uma linha (cultivo)
        }

        /* // comentado já que o botão cancelar faz isso
        protected void AddButtonClick()
        {
            // Ação ao clicar no botão "Adicionar"
            isModoEditar = false;

            textoCadastrarOuEditar = "Cadastrar Cultivo";

            cultivoCadastrarOuEditar = new CultivoDTO();
        }
        */

        protected void CancelarClick()
        {
            // Ação ao clicar no botão "Adicionar"
            isModoEditar = false;

            textoCadastrarOuEditar = "Cadastrar Cultivo";

            cultivoCadastrarOuEditar = new CultivoDTO();
        }

        protected void EditarCultivo(CultivoDTO cultivo)
        {
            AlternarModoEditar();

            // Cria uma cópia do cultivo para edição
            cultivoCadastrarOuEditar = new CultivoDTO
            {
                Id = cultivo.Id,
                Nome = cultivo.Nome,
                Variedade = cultivo.Variedade,
                Categoria = cultivo.Categoria,
                TempoProdTradicional = cultivo.TempoProdTradicional,
                TempoProdControlado = cultivo.TempoProdControlado,
                StatusAtivo = cultivo.StatusAtivo
            };
        }

        protected async Task ExcluirCultivo(CultivoDTO cultivo)
        {
            try
            {
                await CultivoApiService.DeleteAsync(cultivo.Id);
                await LoadCultivos(); // Atualiza a lista após exclusão
                NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Cultivo excluído com sucesso!", duration: 3000);
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro ao excluir", ex.Message, duration: 5000);
            }

        }

        [Inject]
        protected DialogService DialogService { get; set; }

        private async Task ConfirmarExclusao(CultivoDTO cultivo)
        {
            bool? confirm = await DialogService.Confirm($"Tem certeza que deseja excluir {cultivo.Variedade}?",
                                                         "Confirmação de Exclusão",
                                                         new ConfirmOptions { OkButtonText = "Excluir", CancelButtonText = "Cancelar" });

            if (confirm == true)
            {
                await ExcluirCultivo(cultivo);
            }
        }

        protected void AlternarModoEditar()
        {
            if (isModoEditar == false) // se não estiver no modo de edição
            {
                // passar pro modo de edição
                isModoEditar = true;
                textoCadastrarOuEditar = "Editar Cultivo";
            }
            else // se estiver no modo de edição
            {
                // passar pro modo de cadastro
                isModoEditar = false;
                textoCadastrarOuEditar = "Cadastrar Cultivo";
                cultivoCadastrarOuEditar = new CultivoDTO(); // Limpa o formulário
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
            string fileName = $"Cultivos_{DateTime.Now:yyyyMMdd_HHmmss}";

            try
            {
                // Verifique se há dados
                if (cultivos == null || !cultivos.Any())
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", "Não há dados para exportar.", duration: 2000);
                    return;
                }

                // Chama o serviço para exportação com base no formato selecionado
                var fileBytes = await exportacaoApiService.ExportarAsync(cultivos, format, fileName);

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
