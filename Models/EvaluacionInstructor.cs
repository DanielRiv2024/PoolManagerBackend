namespace PoolManagerBackend.Models
{
    public class EvaluacionInstructor
    {
        public int ID_Evaluacion { get; set; }
        public int ID_Cliente { get; set; }
        public int ID_Instructor { get; set; }
        public decimal Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha_evaluacion { get; set; }
    }
}
