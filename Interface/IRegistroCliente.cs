using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface IRegistroCliente
    {
        public Task<List<RegistroCliente>> GetAll();
        public Task<Boolean> Create(RegistroCliente registroCliente);
        public Task<Boolean> Delete(int id);
        public Task<RegistroCliente> Edit(RegistroCliente registroCliente);
    }
}
