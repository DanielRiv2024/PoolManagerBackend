namespace PoolManagerBackend.Models
{
    public class Cliente
    {
        public int ID_Cliente { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Edad { get; set; }
        public string Correo_electronico { get; set; }
        public string Telefono { get; set; }
        public string Contrasena { get; set; }
    }
}
