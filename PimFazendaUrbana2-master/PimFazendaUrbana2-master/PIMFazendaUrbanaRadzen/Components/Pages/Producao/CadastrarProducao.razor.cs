using Microsoft.AspNetCore.Components;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;
using Radzen;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Producao
{
    public partial class CadastrarProducao
    {
        [Inject]
        public ProducaoApiService<ProducaoDTO> ProducaoApiService { get; set; }

        [Inject]
        public CultivoApiService<CultivoDTO> CultivoApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected ProducaoDTO producao;
        protected bool errorVisible;

        protected List<CultivoDTO> cultivos;
        protected CultivoDTO cultivoSelecionado;

        protected int? tempoProd;

        List<string> unidadesPermitidas = new List<string>
        {
            "kg", "g", "unidade"
        };

        protected override async Task OnInitializedAsync()
        {
            cultivos = new List<CultivoDTO>();
            cultivoSelecionado = new CultivoDTO();

            producao = new ProducaoDTO
            {
                Cultivo = new CultivoDTO() // Evita erros de referência nula
            };

            await LoadCultivos();
        }

        protected string errorMessage = string.Empty;

        protected async Task LoadCultivos()
        {
            try
            {
                cultivos = await CultivoApiService.GetAllAsync(); // Carrega todos os cultivos

                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar cultivos: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        protected void AtualizarCultivoSelecionado(object args)
        {
            if (args is CultivoDTO cultivo)
            {
                cultivoSelecionado = cultivo;
                producao.Cultivo = cultivoSelecionado;
                AtualizarTempoProd();
            }
        }

        protected void AtualizarTempoProd()
        {
            if (cultivoSelecionado != null)
            {
                tempoProd = producao.AmbienteControlado
                    ? cultivoSelecionado.TempoProdControlado
                    : cultivoSelecionado.TempoProdTradicional;
            }
            else
            {
                tempoProd = null;
            }

            // Força atualização de DataColheita mesmo sem mudança explícita de Data
            AtualizarDataColheita();
        }

        protected void AtualizarDataColheita()
        {
            if (producao.Data != default && tempoProd.HasValue)
            {
                // Soma o tempo de produção à data inicial
                producao.DataColheita = producao.Data.AddDays(tempoProd.Value);
            }
            else
            {
                producao.DataColheita = default; // Reseta caso não haja informações suficientes
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                producao.StatusFinalizado = false; // Define StatusFinalizado como false por padrão

                Console.WriteLine($"Chamando ApiService: CreateAsync" + " hora atual: " + DateTime.Now);
                var response = await ProducaoApiService.CreateAsync(producao); // Chama ApiService
                Console.WriteLine("Retornou de ApiService: Create Async" + " hora atual: " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Navegando para /producao");
                    // Redireciona para a página de producao e exibe mensagem de sucesso
                    NavigationManager.NavigateTo("/producao");
                    NotificationService.Notify(NotificationSeverity.Success, "Sucesso", "Produção cadastrado com sucesso!", duration: 5000);
                }
                else
                {
                    // Usando ApiResponseHelper apenas para processar resposta de erro
                    var errorMessage = await ApiResponseHelper.HandleErrorResponseAsync(response);
                    NotificationService.Notify(NotificationSeverity.Error, "Erro", $"Falha ao cadastrar produção: {errorMessage}", duration: 10000);
                }
            }
            catch (Exception ex)
            {
                errorVisible = true; // Exibe mensagem de erro
                Console.WriteLine($"Erro ao cadastrar produção: {ex.Message}");
            }
        }


        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de producao
            NavigationManager.NavigateTo("/producao");
        }

        
    }
}
