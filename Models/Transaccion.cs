namespace PoolManagerBackend.Models
{
    public class Transaccion
    {
        public int ID_Pago { get; set; }
        public int ID_Reserva { get; set; }
        public int ID_Tarjeta { get; set; }
        public string Detalle { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha_de_pago { get; set; }
        public bool Estado_de_pago { get; set; } // Cambiado a tipo booleano//ya se quedo asi no funco
        public DateTime Fecha_de_transaccion { get; set; }
        public string Mensaje_de_respuesta { get; set; }
    }
}
