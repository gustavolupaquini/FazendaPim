using Microsoft.AspNetCore.Mvc;
using PIMFazendaUrbanaLib;
using PIMFazendaUrbanaAPI.DTOs;
using AutoMapper;

namespace PIMFazendaUrbanaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducaoController : ControllerBase
    {
        private readonly IProducaoService _producaoService;
        private readonly IMapper _mapper; // Adiciona o AutoMapper

        // O controlador utiliza a interface IProducaoService para acessar as operações de Producao
        public ProducaoController(IProducaoService producaoService, IMapper mapper)
        {
            _producaoService = producaoService;
            _mapper = mapper; // Inicializa o AutoMapper
        }

        [HttpGet("filtradas")]
        public ActionResult<List<ProducaoDTO>> ListarProducoesFiltradas(string search)
        {
            try
            {
                var producao = _producaoService.ListarProducoesComFiltros(search);
                var producaoDto = _mapper.Map<List<ProducaoDTO>>(producao); // Mapeia Producao para ProducaoDTO
                return Ok(producaoDto); // Retorna a lista de Producoes filtradas como resposta
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar produções: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }


        // Método para listar Producao ativos
        [HttpGet("listar")]
        public IActionResult ListarProducoes()
        {
            try
            {
                var producao = _producaoService.ListarProducoes();
                var producaoDto = _mapper.Map<List<ProducaoDTO>>(producao); // Mapeia Producao para ProducaoDTO
                return Ok(producaoDto);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar produções: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }


        // Método para cadastrar um Producao
        [HttpPost("cadastrar")]
        public IActionResult CadastrarProducao([FromBody] ProducaoDTO producaoDto)
        {
            try
            {
                var producao = _mapper.Map<Producao>(producaoDto); // Mapeia Producao para ProducaoDTO
                _producaoService.CadastrarProducao(producao); // Chama o serviço para cadastrar o Producao
                return Ok(new { message = "Produção cadastrada com sucesso." });
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

        // Método para finalizar uma Producao
        [HttpPatch("finalizar/{id}")]
        public IActionResult FinalizarProducao(int id)
        {
            try
            {
                _producaoService.FinalizarProducao(id); // Chama o serviço para excluir o Producao
                return Ok(new { message = "Produção finalizada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        // Método para buscar um Producao por ID
        [HttpGet("{id}")]
        public IActionResult ConsultarProducaoPorId(int id)
        {
            try
            {
                var producao = _producaoService.ConsultarProducaoPorId(id);
                var producaoDto = _mapper.Map<ProducaoDTO>(producao); // Mapeia Producao para ProducaoDTO
                return Ok(producaoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

    }
}
