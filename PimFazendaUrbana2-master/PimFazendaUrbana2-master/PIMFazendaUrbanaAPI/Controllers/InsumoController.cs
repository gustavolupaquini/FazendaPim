using Microsoft.AspNetCore.Mvc;
using PIMFazendaUrbanaLib;
using PIMFazendaUrbanaAPI.DTOs;
using AutoMapper;

namespace PIMFazendaUrbanaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsumoController : ControllerBase
    {
        private readonly IInsumoService _insumoService;
        private readonly IMapper _mapper; // Adiciona o AutoMapper

        // O controlador utiliza a interface IInsumoService para acessar as operações de Insumo
        public InsumoController(IInsumoService insumoService, IMapper mapper)
        {
            _insumoService = insumoService;
            _mapper = mapper; // Inicializa o AutoMapper
        }

        [HttpGet("filtrados")]
        public ActionResult<List<InsumoDTO>> ListarInsumosFiltrados(string search)
        {
            try
            {
                var insumo = _insumoService.ListarInsumosComFiltros(search);
                var insumoDto = _mapper.Map<List<InsumoDTO>>(insumo); // Mapeia Insumo para InsumoDTO
                return Ok(insumoDto); // Retorna a lista de Insumo filtrados como resposta
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar Insumo: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        [HttpGet("saida-filtrados")]
        public ActionResult<List<SaidaInsumoDTO>> ListarSaidaInsumosFiltrados(string search)
        {
            try
            {
                var saidaInsumo = _insumoService.ListarSaidaInsumosComFiltros(search);
                var saidaInsumoDto = _mapper.Map<List<SaidaInsumoDTO>>(saidaInsumo); // Mapeia Insumo para InsumoDTO
                return Ok(saidaInsumoDto); // Retorna a lista de Insumo filtrados como resposta
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar saída de insumos: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }


        // Método para listar Insumo ativos
        [HttpGet("ativos")]
        public IActionResult ListarInsumoAtivos()
        {
            try
            {
                var insumo = _insumoService.ListarInsumosAtivos();
                var insumoDto = _mapper.Map<List<InsumoDTO>>(insumo); // Mapeia Insumo para InsumoDTO
                return Ok(insumoDto);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar Insumo: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        [HttpGet("saida")]
        public IActionResult ListarSaidaInsumo()
        {
            try
            {
                var insumo = _insumoService.ListarSaidaInsumos();
                var insumoDto = _mapper.Map<List<SaidaInsumoDTO>>(insumo); // Mapeia Insumo para InsumoDTO
                return Ok(insumoDto);
            }
            catch (Exception ex)
            {
                // Log detalhado do erro
                Console.WriteLine($"Erro ao listar saidas de insumo: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }


        // Método para cadastrar um Insumo
        [HttpPost("cadastrar")]
        public IActionResult CadastrarInsumo([FromBody] InsumoDTO insumoDto)
        {
            try
            {
                var insumo = _mapper.Map<Insumo>(insumoDto); // Mapeia Insumo para InsumoDTO
                _insumoService.CadastrarInsumo(insumo); // Chama o serviço para cadastrar o Insumo
                return Ok(new { message = "Insumo cadastrado com sucesso." });
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

        // Método para alterar um Insumo
        [HttpPut("alterar")]
        public IActionResult AlterarInsumo([FromBody] InsumoDTO insumoDto)
        {
            try
            {
                var insumo = _mapper.Map<Insumo>(insumoDto); // Mapeia InsumoDTO para Insumo
                _insumoService.AlterarInsumo(insumo); // Chama o serviço para alterar o Insumo
                return Ok(new { message = "Insumo alterado com sucesso." });
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

        [HttpPost("baixar-insumo")]
        public IActionResult BaixarInsumo([FromBody] SaidaInsumoDTO saidaInsumoDto)
        {
            try
            {
                var saidaInsumo = _mapper.Map<SaidaInsumo>(saidaInsumoDto); // Mapeia InsumoDTO para Insumo

                _insumoService.CadastrarSaidaInsumo(saidaInsumo); // Chama o serviço para alterar o Insumo
                return Ok(new { message = "Insumo alterado com sucesso." });
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

        // Método para excluir um Insumo (exclusão lógica)
        [HttpDelete("excluir/{id}")]
        public IActionResult DesativarInsumo(int id)
        {
            try
            {
                _insumoService.DesativarInsumo(id); // Chama o serviço para excluir o Insumo
                return Ok(new { message = "Insumo excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        // Método para buscar um Insumo por ID
        [HttpGet("{id}")]
        public IActionResult ConsultarInsumoPorId(int id)
        {
            try
            {
                var insumo = _insumoService.ConsultarInsumoPorId(id);
                var insumoDto = _mapper.Map<InsumoDTO>(insumo); // Mapeia Insumo para InsumoDTO
                return Ok(insumoDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

    }
}
