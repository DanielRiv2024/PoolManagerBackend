using PoolManagerBackend.Models;

namespace PoolManagerBackend.Interface
{
    public interface ITarjetaUsuario
    {
        public Task<List<TarjetaUsuario>> GetAll();
        public Task<Boolean> Create(TarjetaUsuario tarjetaUsuario);
        public Task<Boolean> Delete(int id);
        public Task<TarjetaUsuario> Edit(TarjetaUsuario tarjetaUsuario);
        public Task<string> VerificarTarjetas();

    }
}
