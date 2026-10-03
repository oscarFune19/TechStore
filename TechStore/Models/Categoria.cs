using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class Categoria
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int ID { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; } = string.Empty;
        [Required(ErrorMessage = "La imagen es requerida")]
        public string Imagen { get; set; } = string.Empty;
        [Required(ErrorMessage = "El estado es requerido")]
        public bool Activa { get; set; } = true;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
