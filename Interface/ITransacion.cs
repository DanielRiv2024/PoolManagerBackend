using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface ITransacion
    {
        public Task<List<Transaccion>> GetAll();
        public Task<bool> Delete(int id);
        public Task<Transaccion> Update(Transaccion transaccion);
        public Task<Transaccion> Create(Transaccion transaccion);
    }
}
