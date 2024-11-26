using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Insumos
{
    public partial class BaixarInsumo
    {
        [Parameter]
        public int InsumoId { get; set; }

        [Inject]
        public InsumoApiService<InsumoDTO> InsumoApiService { get; set; }

        [Inject]
        public InsumoApiService<SaidaInsumoDTO> SaidaInsumoApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected InsumoDTO insumo;
        protected SaidaInsumoDTO saidaInsumo;
        protected int qtdSaida;
        protected bool errorVisible;

        protected List<string> unidades = new List<string>
        {
            "kg", "g", "l", "ml", "m", "cm", "mm", "unidade", "caixa", "tambor"
        };

        protected List<string> categoria = new List<string>
        {
            "Sementes", "Fertilizantes"
        };

        protected override async Task OnInitializedAsync()
        {
            insumo = new InsumoDTO();
            saidaInsumo = new SaidaInsumoDTO();
            await LoadInsumo();
        }

        protected async Task LoadInsumo()
        {
            try
            {
                insumo = await InsumoApiService.GetByIdAsync(InsumoId);

                if (insumo == null)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Aviso", "Insumo não encontrado. Redirecionando para a lista de insumos.", duration: 5000);
                    NavigationManager.NavigateTo("/insumos");
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Erro ao carregar insumos: {ex.Message}", duration: 5000);
                NavigationManager.NavigateTo("/insumos");
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                if (qtdSaida > insumo.Qtd || qtdSaida == 0)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Quantidade não pode ser maior que a de estoque atual", duration: 5000);
                    return;
                }

                saidaInsumo.Qtd = qtdSaida;
                saidaInsumo.Unidqtd = insumo.Unidqtd;
                saidaInsumo.NomeInsumo = insumo.Nome;
                saidaInsumo.CategoriaInsumo = insumo.Categoria;
                saidaInsumo.Data = DateTime.Now;
                saidaInsumo.IdInsumo = insumo.Id;

                Console.WriteLine($"Chamando ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);
                var response = await SaidaInsumoApiService.DecreaseAsync(saidaInsumo);
                Console.WriteLine("Retornou de ApiService: UpdateAsync" + " hora atual: " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Navegando para /insumos");
                    // Redireciona para a página de insumos e exibe mensagem de sucesso
                    NavigationManager.NavigateTo("/insumos");
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Insumo atualizado com sucesso!", duration: 5000);
                }
                else
                {
                    // Usando ApiResponseHelper apenas para processar resposta de erro
                    var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao atualizar insumo: {errorMessage}", duration: 10000);
                }
            }
            catch (Exception ex)
            {
                errorVisible = true; // Exibe mensagem de erro
                Console.WriteLine($"Erro ao atualizar insumo: {ex.Message}");
            }
        }


        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de clientes
            NavigationManager.NavigateTo("/insumos");
        }
    }
}
