using PoolManagerBackend.Models;
using System.Security.Cryptography.Xml;

namespace PoolManagerBackend.Interface
{
    public interface IComentario
    {

        public Task<List<Comentario>> GetAll();
        public Task<Boolean> Create(Comentario comentario);
        public Task<Boolean> Delete(int id);
        public Task<Comentario> Edit(Comentario comentario);
        public Task<string> MostrarComentariosCliente(int idCliente);
    }
}
