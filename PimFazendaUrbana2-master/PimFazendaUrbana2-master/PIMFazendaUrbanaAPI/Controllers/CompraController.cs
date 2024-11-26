using Microsoft.AspNetCore.Mvc;
using PIMFazendaUrbanaLib;
using PIMFazendaUrbanaAPI.DTOs;
using AutoMapper;

namespace PIMFazendaUrbanaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;
        private readonly IMapper _mapper; // Adiciona o AutoMapper

        // O controlador utiliza a interface ICompraService para acessar as operações de compra
        public CompraController(ICompraService compraService, IMapper mapper)
        {
            _compraService = compraService;
            _mapper = mapper; // Inicializa o AutoMapper
        }

        [HttpGet("filtrados")]
        public ActionResult<List<PedidoCompraItemDTO>> ListarPedidoCompraItensComFiltros(string search)
        {
            try
            {
                var pedidoCompraItens = _compraService.ListarPedidoCompraItensComFiltros(search);
                var pedidoCompraItensDto = _mapper.Map<List<PedidoVendaItemDTO>>(pedidoCompraItens); // Mapeia PedidoVendaItem para PedidoVendaItemDTO
                return Ok(pedidoCompraItensDto); // Retorna a lista de compras filtradas como resposta
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar compras: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        // Método para listar compras
        [HttpGet("listar")]
        public IActionResult ListarRegistrosDeCompra()
        {
            try
            {
                var pedidoCompraItens = _compraService.ListarRegistrosDeCompra();
                var pedidoCompraItensDto = _mapper.Map<List<PedidoCompraItemDTO>>(pedidoCompraItens); // Mapeia PedidoCompraItem para PedidoCompraItemDTO
                return Ok(pedidoCompraItensDto);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar compras: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        // Método para cadastrar um compra
        [HttpPost("cadastrar")]
        public IActionResult CadastrarCompra([FromBody] PedidoCompraDTO pedidoCompraDto)
        {
            try
            {
                var pedidoCompra = _mapper.Map<PedidoCompra>(pedidoCompraDto); // Mapeia PedidoCompraDTO para PedidoCompra
                _compraService.CadastrarPedidoCompra(pedidoCompra); // Chama o serviço para cadastrar o compra
                return Ok(new { message = "Compra cadastrada com sucesso." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors }); // Retorna erros de validação
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

    }
}
