namespace PoolManagerBackend.Models
{
    public class RegistroCliente
    {
        public int ID_Actividad { get; set; }
        public int ID_Cliente { get; set; }
        public string Tipo_Actividad { get; set; }
        public DateTime Fecha_hora_inicio { get; set; }
        public DateTime Fecha_hora_fin { get; set; }
    }
}
