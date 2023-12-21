using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoolManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroController : ControllerBase
    {
        private readonly IRegistroCliente _registroService;

        public RegistroController(IRegistroCliente registroService)
        {
            _registroService = registroService;
        }

        [HttpGet("GetAllActividadesCliente")]
        public async Task<IActionResult> GetAllActividadesCliente()
        {
            try
            {
                List<RegistroCliente> actividadesClientes = await _registroService.GetAll();
                return Ok(actividadesClientes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpPost("CreateActividadCliente")]
        public async Task<IActionResult> CreateActividadCliente(RegistroCliente registroCliente)
        {
            try
            {
                var status = await _registroService.Create(registroCliente);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error crítico: {ex.Message}");
            }
        }

        [HttpPut("EditActividadCliente")]
        public async Task<IActionResult> EditActividadCliente(RegistroCliente registroCliente)
        {
            try
            {
                var updatedRegistroCliente = await _registroService.Edit(registroCliente);

                if (updatedRegistroCliente != null)
                {
                    return Ok(updatedRegistroCliente);
                }
                else
                {
                    return NotFound($"RegistroCliente with ID {registroCliente.ID_Actividad} not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error crítico: {ex.Message}");
            }
        }

        [HttpDelete("DeleteActividadCliente/{id}")]
        public async Task<IActionResult> DeleteActividadCliente(int id)
        {
            var actividadClienteEliminada = await _registroService.Delete(id);

            if (actividadClienteEliminada)
            {
                return Ok($"RegistroCliente with ID {id} deleted successfully.");
            }
            else
            {
                return NotFound($"RegistroCliente with ID {id} not found.");
            }
        }
    }
}
