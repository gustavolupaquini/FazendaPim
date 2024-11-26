using Microsoft.AspNetCore.Components;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Insumos
{
    public partial class CadastrarInsumo
    {
        [Inject]
        public InsumoApiService<InsumoDTO> InsumoApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected InsumoDTO insumo;
        protected bool errorVisible;

        protected override async Task OnInitializedAsync()
        {
            insumo = new InsumoDTO
            {
               
            };
        }

        protected List<string> unidades = new List<string>
        {
            "kg", "g", "l", "ml", "m", "cm", "mm", "unidade", "caixa", "tambor"
        };

        protected List<string> categoria = new List<string>
        {
            "Sementes", "Fertilizantes"
        };

        protected async Task FormSubmit()
        {
            try
            { 

                insumo.Ativo = true; // Define StatusAtivo como true por padrão
                insumo.Qtd = 0;

                Console.WriteLine($"Chamando ApiService: CreateAsync" + " hora atual: " + DateTime.Now);
                var response = await InsumoApiService.CreateAsync(insumo); // Chama ApiService para criar o insumo
                Console.WriteLine("Retornou de ApiService: Create Async" + " hora atual: " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Navegando para /insumos");
                    // Redireciona para a página de insumos e exibe mensagem de sucesso
                    NavigationManager.NavigateTo("/insumos");
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Insumo cadastrado com sucesso!", duration: 5000);
                }
                else
                {
                    // Usando ApiResponseHelper apenas para processar resposta de erro
                    var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao cadastrar insumo: {errorMessage}", duration: 10000);
                }
            }
            catch (Exception ex)
            {
                errorVisible = true; // Exibe mensagem de erro
                Console.WriteLine($"Erro ao cadastrar insumo: {ex.Message}");
            }
        }

        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de insumos
            NavigationManager.NavigateTo("/insumos");
        }

    }
}
