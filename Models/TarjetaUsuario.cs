using PoolManagerBackend.Models;

namespace PoolManagerBackend.Models
{
    public class TarjetaUsuario
    {
        public int ID_Tarjeta { get; set; }
        public int ID_Cliente { get; set; }
        public string Numero_de_tarjeta { get; set; }
        public string Nombre_en_la_tarjeta { get; set; }
        public DateTime Fecha_de_expiracion { get; set; }
        public string CVV { get; set; }
        public int Cliente { get; set; }
    }
}
