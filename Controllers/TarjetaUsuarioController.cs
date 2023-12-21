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
    public class TarjetaUsuarioController : ControllerBase
    {
        private readonly ITarjetaUsuario _tarjetaUsuarioService;

        public TarjetaUsuarioController(ITarjetaUsuario tarjetaUsuarioService)
        {
            _tarjetaUsuarioService = tarjetaUsuarioService;
        }

        [HttpGet("GetTarjetasUsuario")]
        public IActionResult Get()
        {
            try
            {
                List<TarjetaUsuario> tarjetasUsuario = _tarjetaUsuarioService.GetAll().Result;

                return Ok(tarjetasUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpPost("CreateTarjetaUsuario")]
        public async Task<IActionResult> CreateTarjetaUsuario(TarjetaUsuario tarjetaUsuario)
        {
            try
            {
                var status = await _tarjetaUsuarioService.Create(tarjetaUsuario);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return Ok("Error Crítico: " + ex);
            }
        }

        [HttpPut("EditTarjetaUsuario")]
        public async Task<IActionResult> EditTarjetaUsuario(TarjetaUsuario tarjetaUsuario)
        {
            try
            {
                var status = await _tarjetaUsuarioService.Edit(tarjetaUsuario);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return Ok("Error Crítico: " + ex);
            }
        }

        [HttpDelete("DeleteTarjetaUsuario")]
        public async Task<IActionResult> DeleteTarjetaUsuario([FromBody] Delete request)
        {
            var tarjetaUsuarioEliminada = await _tarjetaUsuarioService.Delete(request.id);
            return Ok(tarjetaUsuarioEliminada);
        }

        [HttpGet("CRSVerificarTarjetas")]
        public async Task<IActionResult> VerificarTarjetas()
        {
            try
            {
                var resultado = await _tarjetaUsuarioService.VerificarTarjetas();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al verificar tarjetas: {ex.Message}");
            }
        }
    }
}