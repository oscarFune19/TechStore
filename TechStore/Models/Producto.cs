using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStore.Models
{
    public class Producto
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int ID { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; } = string.Empty;
        [Required(ErrorMessage = "El precio es requerido")]
        public decimal Precio { get; set; }
        [Required(ErrorMessage = "La categoría es requerida")]
        [NotMapped]
        public string Categoria { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public Categoria? CategoriaEntidad { get; set; }
        [Required(ErrorMessage = "La imagen es requerida")]
        public string Imagen { get; set; } = string.Empty;
        [Required(ErrorMessage = "Porfavor indique cuanto es el stock existente")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "Porfavor coloque el estado")]
        public bool Estado { get; set; }
    }
}