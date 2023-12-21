using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface IPiscinaService
    {
        public Task<List<Piscina>> GetAllPiscina();
        public Task<bool> DeletePiscina(int id);
        public Task<bool> UpdatePiscina(Piscina piscina);
        public Task<bool> CreatePiscina(Piscina piscina);
        public Task<string> ListarPiscinasCapacidadMaxima();
    }
}
