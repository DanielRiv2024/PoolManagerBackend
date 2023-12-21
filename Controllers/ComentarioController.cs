using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using PoolManagerBackend.Services;

namespace PoolManagerBackend.Controllers
{
    [Route("Oracle/[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        private readonly IComentario _comentario;
        public ComentarioController(IComentario comentario)
        {
            _comentario = comentario;
        }

        [HttpGet("GetComentarios")]
        public IActionResult Get()
        {
            try
            {
                List<Comentario> comentarios = _comentario.GetAll().Result;

                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }
        [HttpPost("CreateComentario")]
        public async Task<OkObjectResult> CreateClient(Comentario comentario)
        {
            try
            {
                var status = await _comentario.Create(comentario);
                return Ok(status);

            }
            catch (Exception ex)
            {
                return Ok("Error Critico" + ex);
            }

        }
        [HttpPut("EditComentario")]
        public async Task<OkObjectResult> EditClient(Comentario comentarios)
        {
            try
            {
                var status = await _comentario.Edit(comentarios);
                return Ok(status);

            }
            catch (Exception ex)
            {
                return Ok("Error Critico" + ex);
            }
        }
        [HttpDelete("DeleteComentario")]
        public async Task<IActionResult> Dlete([FromBody] Delete request)
        {
            var comentarioEliminado = await _comentario.Delete(request.id);

            return Ok(comentarioEliminado);
        }

        [HttpGet("CRSMostrarComentariosCliente/{idCliente}")]
        public async Task<IActionResult> GetMostrarComentariosCliente(int idCliente)
        {
            try
            {
                var resultado = await _comentario.MostrarComentariosCliente(idCliente);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener comentarios del cliente: {ex.Message}");
            }
        }

    }
 
}

