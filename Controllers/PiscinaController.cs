using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;

namespace PoolManagerBackend.Controllers
{
    [Route("Oracle/[controller]")]
    [ApiController]
    public class PiscinaController : ControllerBase
    {
        private readonly IPiscinaService _piscinaService;
        public PiscinaController(IPiscinaService piscinaService)
        {
            _piscinaService = piscinaService;
        }
        [HttpGet("GetPiscinas")]
        public IActionResult Get()
        {
            try
            {
                List<Piscina> clientes = _piscinaService.GetAllPiscina().Result;

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }
        [HttpPost("CreatePiscina")]
        public async Task<OkObjectResult> CreateClient(Piscina piscina)
        {
            try
            {
                var status = await _piscinaService.CreatePiscina(piscina);
                return Ok(status);

            }
            catch (Exception ex)
            {
                return Ok("Error Critico" + ex);
            }

        }
        [HttpPut("EditPiscina")]
        public async Task<OkObjectResult> EditClient(Piscina piscina)
        {
            try
            {
                var status = await _piscinaService.UpdatePiscina(piscina);
                return Ok(status);

            }
            catch (Exception ex)
            {
                return Ok("Error Critico" + ex);
            }
        }
        [HttpDelete("DeletePiscina")]
        public async Task<IActionResult> Delete([FromBody] Delete request)
        {
            var clienteEliminada = await _piscinaService.DeletePiscina(request.id);

            return Ok(clienteEliminada);
        }
        [HttpGet("CRSListadoCapacidad")]
        public async Task<IActionResult> GetListarPiscinasCapacidadMaxima()
        {
            try
            {
                var resultado = await _piscinaService.ListarPiscinasCapacidadMaxima();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la lista de piscinas con capacidad máxima: {ex.Message}");
            }
        }
    }
    public class Delete
    {
        public int id { get; set; }
    }
}

