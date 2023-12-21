using PoolManagerBackend.Models;

namespace PoolManagerBackend.Models
{
    public class Reserva
    {
        public int ID_Reserva { get; set; }
        public int ID_Cliente { get; set; }
        public int ID_Piscina { get; set; }
        public int ID_Clase { get; set; }
        public DateTime Fecha_hora_inicio { get; set; }
        public DateTime Fecha_hora_fin { get; set; }
        public decimal Costo { get; set; }
        public string Pago_en_linea { get; set; }
    }
}