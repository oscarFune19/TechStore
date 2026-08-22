namespace TechStore.Models
{
    public class Categoria
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;
    }
}
