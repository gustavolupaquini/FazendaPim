using Microsoft.AspNetCore.Mvc;
using PIMFazendaUrbanaLib;
using PIMFazendaUrbanaAPI.DTOs;
using AutoMapper;

namespace PIMFazendaUrbanaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueProdutoController : ControllerBase
    {
        private readonly IEstoqueProdutoService _estoqueProdutoService;
        private readonly IMapper _mapper; // Adiciona o AutoMapper

        // O controlador utiliza a interface IEstoqueProdutoService para acessar as operações de Estoque
        public EstoqueProdutoController(IEstoqueProdutoService estoqueProdutoService, IMapper mapper)
        {
            _estoqueProdutoService = estoqueProdutoService;
            _mapper = mapper; // Inicializa o AutoMapper
        }

        [HttpGet("filtrados")]
        public ActionResult<List<EstoqueProdutoDTO>> ListarEstoqueProdutoFiltrados(string search)
        {
            try
            {
                var estoqueProduto = _estoqueProdutoService.ListarEstoqueProdutoComFiltros(search);
                var estoqueProdutoDto = _mapper.Map<List<EstoqueProdutoDTO>>(estoqueProduto); // Mapeia EstoqueProduto para EstoqueProdutoDTO
                return Ok(estoqueProdutoDto); // Retorna a lista de EstoqueProduto filtrados como resposta
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar Estoques: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }


        // Método para listar EstoqueProduto ativos
        [HttpGet("ativos")]
        public IActionResult ListarEstoqueProdutoAtivo()
        {
            try
            {
                var estoqueProduto = _estoqueProdutoService.ListarEstoqueProdutoAtivos();
                var estoqueProdutoDto = _mapper.Map<List<EstoqueProdutoDTO>>(estoqueProduto); // Mapeia EstoqueProduto para EstoqueProdutoDTO
                return Ok(estoqueProdutoDto);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar Estoque: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }
    }
}
