using Microsoft.AspNetCore.Mvc;
using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface IReservaService
    {
        public Task<List<Reserva>> GetAllReserva();
        public Task<bool> DeleteReserva(int id);
        public Task<bool> CreateReserva(Reserva reserva);
        public Task<bool> UpdateReserva(Reserva reserva);
        public Task<string> GetTotalIngresosReservas();
        public Task<string> ObtenerReservasPagadasEnLinea();
    }
}
