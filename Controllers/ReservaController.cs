using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Types;
using PoolManagerBackend.Interface;
using PoolManagerBackend.Models;
using static PoolManagerBackend.Controllers.ClienteController;

namespace PoolManagerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        public ReservaController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }
        [HttpGet("reservations")]
        public async Task<IActionResult> Get()
        {
            var reservas = await _reservaService.GetAllReserva();
            return Ok(reservas);
        }
        [HttpDelete("deleteReservation")]
        public async Task<IActionResult> Delete([FromBody] DeleteParameter req)
        {
            var reservaEliminada = await _reservaService.DeleteReserva(req.id);

            return Ok(reservaEliminada);
        }
        [HttpPost("CreateReservation")]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            try
            {
                var reservaCreadaResult = await _reservaService.CreateReserva(reserva);

                return reservaCreadaResult switch
                {
                    true => Ok("Reserva creada exitosamente"),
                    false => BadRequest("No se pudo crear la reserva")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpPut("EditReservation")]
        public async Task<IActionResult> Edit(Reserva reserva)
        {
            try
            {
                var reservaCreadaResult = await _reservaService.UpdateReserva(reserva);

                return reservaCreadaResult switch
                {
                    true => Ok("Reserva actualizada exitosamente"),
                    false => BadRequest("No se pudo actualizada la reserva")
                };
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");

                return StatusCode(500, "Error interno del servidor");
            }
        }
        [HttpGet("CRSTotalIngresos")]
        public async Task<IActionResult> GetTotalIngresos()
        {
            try
            {
                var totalIngresos = await _reservaService.GetTotalIngresosReservas();

                return Ok(totalIngresos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("CRSReservasPagadasEnLinea")]
        public async Task<IActionResult> GetReservasPagadasEnLinea()
        {
            try
            {
                string reservasPagadas = await _reservaService.ObtenerReservasPagadasEnLinea();
                return Ok(reservasPagadas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener reservas pagadas en línea: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


    }
}
public class DeleteParameter
{
    public int id { get; set; }
}
