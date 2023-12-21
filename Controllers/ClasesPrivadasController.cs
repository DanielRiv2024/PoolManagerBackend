using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoolManagerBackend.Controllers
{
    [Route("Oracle/[controller]")]
    [ApiController]
    public class ClasePrivadaController : ControllerBase
    {
        private readonly IClasesPrivadasService _clasePrivadaService;

        public ClasePrivadaController(IClasesPrivadasService clasePrivadaService)
        {
            _clasePrivadaService = clasePrivadaService;
        }

        [HttpGet("GetClasesPrivadas")]
        public IActionResult Get()
        {
            try
            {
                List<ClasePrivada> clasesPrivadas = _clasePrivadaService.GetAll().Result;

                return Ok(clasesPrivadas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpGet("GetClasePrivadaById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                ClasePrivada clasePrivada = await _clasePrivadaService.GetById(id);

                if (clasePrivada != null)
                {
                    return Ok(clasePrivada);
                }
                else
                {
                    return NotFound($"No se encontró ninguna clase privada con el ID {id}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }


        [HttpPost("CreateClasePrivada")]
        public async Task<IActionResult> CreateClasePrivada(ClasePrivada clasePrivada)
        {
            try
            {
                var status = await _clasePrivadaService.Create(clasePrivada);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return Ok("Error Crítico: " + ex);
            }
        }

        [HttpPut("EditClasePrivada")]
        public async Task<IActionResult> EditClasePrivada(ClasePrivada clasePrivada)
        {
            try
            {
                var status = await _clasePrivadaService.Edit(clasePrivada);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return Ok("Error Crítico: " + ex);
            }
        }

        [HttpDelete("DeleteClasePrivada")]
        public async Task<IActionResult> DeleteClasePrivada([FromBody] Delete request)
        {
            var clasePrivadaEliminada = await _clasePrivadaService.Delete(request.id);
            return Ok(clasePrivadaEliminada);
        }

        [HttpGet("CRSCantidadMinutosCPTOTAL")]
        public async Task<IActionResult> GetDuracionTotalClasesPrivadas()
        {
            try
            {
                var duracionTotal = await _clasePrivadaService.GetTotalDuracionClasesPrivadas();

                return Ok(duracionTotal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la duración total de las clases privadas: {ex.Message}");
            }
        }
        [HttpGet("CRSListadoPrecios")]
        public async Task<IActionResult> GetListarClasesPrivadasPrecio()
        {
            try
            {
                var resultado = await _clasePrivadaService.ListarClasesPrivadasPrecio();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la lista de clases privadas con precio: {ex.Message}");
            }
        }
    }
}
