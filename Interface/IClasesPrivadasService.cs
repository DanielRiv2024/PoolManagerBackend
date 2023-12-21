using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface IClasesPrivadasService
    {
        public Task<List<ClasePrivada>> GetAll();
        public Task<Boolean> Create(ClasePrivada clasePrivada);
        public Task<Boolean> Delete(int id);
        public Task<ClasePrivada> Edit(ClasePrivada clasePrivada);
        public Task<ClasePrivada> GetById(int id);
        public Task<string> GetTotalDuracionClasesPrivadas();
        public Task<string> ListarClasesPrivadasPrecio();
    }
}
