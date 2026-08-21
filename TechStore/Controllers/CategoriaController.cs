using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class CategoriasController : Controller
    {
        private static readonly List<Categoria> categorias = new()
        {
            new Categoria
            {
                ID = 1,
                Nombre = "Computadoras",
                Descripcion = "Portátiles y equipos de escritorio para estudio, trabajo y gaming.",
                Imagen = "/images/categorias/computadoras.jpg",
                Activa = true
            },
            new Categoria
            {
                ID = 2,
                Nombre = "Smartphones",
                Descripcion = "Teléfonos móviles de última generación y accesorios.",
                Imagen = "/images/categorias/smartphones.jpg",
                Activa = true
            },
            new Categoria
            {
                ID = 3,
                Nombre = "Audio",
                Descripcion = "Auriculares, altavoces y equipos para una experiencia sonora superior.",
                Imagen = "/images/categorias/audio.jpg",
                Activa = true
            },
            new Categoria
            {
                ID = 4,
                Nombre = "Gaming",
                Descripcion = "Periféricos, monitores y componentes para juegos.",
                Imagen = "/images/categorias/gaming.jpg",
                Activa = true
            }
        };

        public IActionResult Index()
        {
            return View(categorias);
        }
    }
}