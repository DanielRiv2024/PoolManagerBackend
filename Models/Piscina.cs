namespace PoolManagerBackend.Models
{
    public class Piscina
    {
        public int ID_Piscina { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public DateTime Horario_apertura { get; set; }
        public DateTime Horario_cierre { get; set; }
        public int Capacidad_maxima { get; set; }
        public decimal Costo { get; set; }
    }
}
