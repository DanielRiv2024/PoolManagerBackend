using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using PoolManagerBackend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoolManagerBackend.Controllers
{
    [Route("Oracle/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("GetClientes")]
        public IActionResult Get()
        {
            try
            {
                List<Cliente> clientes = _clienteService.GetAllCliente().Result;

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }


        [HttpGet("GetClientesBy/{id}")]
        public async Task<IActionResult> GetByIdC(int id)
        {
            try
            {
                Cliente c = await _clienteService.GetById(id);

                if (c != null)
                {
                    return Ok(c);
                }
                else
                {
                    return NotFound($"No se encontró ningun cliente con el ID {id}");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la conexión: {ex.Message}");
            }
        }

        [HttpDelete("DeleteCliente")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            var clienteEliminada = await _clienteService.DeleteCliente(request.id);

            return Ok(clienteEliminada);
        }
        [HttpPost("Authentication")]
        public IActionResult Authentication([FromBody] AuthenticationRequest request)
        {
            try
            {
                Cliente authenticatedCliente = _clienteService.AuthenticacionSession(request.Username, request.Password).Result;

                if (authenticatedCliente != null)
                {
                   
                    return Ok(authenticatedCliente);
                }
                else
                {
                    return Unauthorized("Nombre de usuario o contraseña incorrectos");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error en la autenticación: {ex.Message}");
            }
        }

        [HttpPut("EditClient")]
        public async Task<OkObjectResult> EditClient(Cliente cliente)
        {
            try
            {
                var status = await _clienteService.UpdateCliente(cliente);
                return Ok(status);
                
            }
            catch (Exception ex)
            {
                return Ok("Error Critico"+ex);
            }
            
        }
        [HttpPost("CreateClient")]
        public async Task<OkObjectResult> CreateClient(Cliente cliente)
        {
            try
            {
                var status = await _clienteService.CreateCliente(cliente);
                return Ok(status);

            }
            catch (Exception ex)
            {
                return Ok("Error Critico" + ex);
            }

        }

        [HttpGet("CRSObtenerPromedioEdadClientes")]
        public async Task<IActionResult> ObtenerPromedioEdadClientes()
        {
            try
            {
                string promedioEdad = await _clienteService.ObtenerPromedioEdadClientes();
                return Ok(promedioEdad);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener el promedio de edad de los clientes: {ex.Message}");
            }
        }

        public class AuthenticationRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        public class DeleteRequest
        {
            public int id { get; set; }
        }
    }
}
