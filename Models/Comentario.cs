namespace PoolManagerBackend.Models
{
    public class Comentario
    {
        public int ID_Comentario { get; set; }
        public int ID_Cliente { get; set; }
        public string Detalle { get; set; }
        public DateTime Fecha_comentario { get; set; }
    }
}
