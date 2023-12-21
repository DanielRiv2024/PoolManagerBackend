using PoolManagerBackend.Models;
using System.Data;
using System.Drawing;

namespace PoolManagerBackend.Interface
{
    public interface IClienteService
    {
        public Task<Cliente> AuthenticacionSession(string user, string pass);
        public Task<bool> DeleteCliente(int id);
        public Task<List<Cliente>> GetAllCliente();
        public Task<bool> UpdateCliente(Cliente cliente);
        public Task<bool> CreateCliente(Cliente cliente);
        public Task<Cliente> GetById(int it);
        public Task<string> ObtenerPromedioEdadClientes();
    }
}
