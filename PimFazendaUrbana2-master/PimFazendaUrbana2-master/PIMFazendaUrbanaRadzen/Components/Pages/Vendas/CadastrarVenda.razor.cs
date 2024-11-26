using Microsoft.AspNetCore.Components;
using Radzen;
using PIMFazendaUrbanaAPI.DTOs;
using PIMFazendaUrbanaRadzen.Services;

namespace PIMFazendaUrbanaRadzen.Components.Pages.Vendas
{
    public partial class CadastrarVenda
    {
        [Inject]
        public VendaApiService<PedidoVendaItemDTO> VendaApiService { get; set; }

        [Inject]
        public EstoqueProdutoApiService<EstoqueProdutoDTO> EstoqueProdutoApiService { get; set; }

        [Inject]
        public ClienteApiService<ClienteDTO> ClienteApiService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public NotificationService NotificationService { get; set; }

        protected PedidoVendaDTO pedidoVenda;

        protected List<PedidoVendaItemDTO> pedidoVendaItens;

        protected PedidoVendaItemDTO pedidoVendaItem;

        protected bool errorVisible;

        protected List<EstoqueProdutoDTO> estoqueProdutos;
        protected EstoqueProdutoDTO estoqueProdutoSelecionado;

        protected List<ClienteDTO> clientes;
        protected ClienteDTO clienteSelecionado;

        protected override async Task OnInitializedAsync()
        {
            clientes = new List<ClienteDTO>();
            foreach (var cliente in clientes)
            {
                cliente.Endereco = new EnderecoDTO();
                cliente.Telefone = new TelefoneDTO();
            }

            clienteSelecionado = new ClienteDTO
            {
                Endereco = new EnderecoDTO(),
                Telefone = new TelefoneDTO()
            };

            estoqueProdutos = new List<EstoqueProdutoDTO>();
            foreach (var estoqueProduto in estoqueProdutos)
            {
                estoqueProduto.Producao = new ProducaoDTO();
                estoqueProduto.Producao.Cultivo = new CultivoDTO();
            }

            estoqueProdutoSelecionado = new EstoqueProdutoDTO
            {
                Producao = new ProducaoDTO
                {
                    Cultivo = new CultivoDTO()
                }
            };

            pedidoVenda = new PedidoVendaDTO
            {
                Itens = new List<PedidoVendaItemDTO>()
            };

            pedidoVendaItens = new List<PedidoVendaItemDTO>();

            await LoadClientes();
            await LoadEstoqueProdutos();
        }

        protected string errorMessage = string.Empty;

        protected async Task LoadClientes()
        {
            try
            {
                clientes = await ClienteApiService.GetAllAsync(); // Carrega todos os clientes

                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar clientes: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        protected async Task LoadEstoqueProdutos()
        {
            try
            {
                estoqueProdutos = await EstoqueProdutoApiService.GetAllAsync(); // Carrega todos os produtos em estoque

                errorMessage = string.Empty; // Limpa mensagens de erro
            }
            catch (Exception ex)
            {
                errorMessage = $"Erro ao carregar produtos em estoque: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        protected void AtualizarClienteSelecionado(object args)
        {
            if (args is ClienteDTO cliente)
            {
                clienteSelecionado = cliente;
                pedidoVenda.IdCliente = clienteSelecionado.Id;
            }
        }

        protected void AtualizarEstoqueProdutoSelecionado(object args)
        {
            if (args is EstoqueProdutoDTO estoqueProduto)
            {
                estoqueProdutoSelecionado = estoqueProduto;
            }
        }

        private int quantidade;
        private decimal valorUnitario;


        protected async Task CancelButtonClick()
        {
            // Redireciona para a página de vendas
            NavigationManager.NavigateTo("/vendas");
        }
    }
}