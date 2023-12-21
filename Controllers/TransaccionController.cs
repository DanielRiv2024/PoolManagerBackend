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
    public class TransaccionController : ControllerBase
    {
        private readonly ITransacion _transaccionService;

        public TransaccionController(ITransacion transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet("GetTransacciones")]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Transaccion> transacciones = await _transaccionService.GetAll();

                return Ok(transacciones);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpPost("CreateTransaccion")]
        public async Task<IActionResult> CreateTransaccion(Transaccion transaccion)
        {
            try
            {
                Transaccion createdTransaccion = await _transaccionService.Create(transaccion);
                return Ok(createdTransaccion);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear Transaccion: {ex.Message}");
            }
        }

        [HttpPut("EditTransaccion")]
        public async Task<IActionResult> EditTransaccion(Transaccion transaccion)
        {
            try
            {
                Transaccion updatedTransaccion = await _transaccionService.Update(transaccion);
                return Ok(updatedTransaccion);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al editar Transaccion: {ex.Message}");
            }
        }

        [HttpDelete("DeleteTransaccion")]
        public async Task<IActionResult> DeleteTransaccion([FromBody] int id)
        {
            try
            {
                bool result = await _transaccionService.Delete(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar Transaccion: {ex.Message}");
            }
        }
    }
}
