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
                Imagen = "https://images.unsplash.com/photo-1499951360447-b19be8fe80f5?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Activa = true
            },
            new Categoria
            {
                ID = 2,
                Nombre = "Smartphones",
                Descripcion = "Teléfonos móviles de última generación y accesorios.",
                Imagen = "https://plus.unsplash.com/premium_photo-1680985551009-05107cd2752c?q=80&w=1332&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Activa = true
            },
            new Categoria
            {
                ID = 3,
                Nombre = "Audio",
                Descripcion = "Auriculares, altavoces y equipos para una experiencia sonora superior.",
                Imagen = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Activa = true
            },
            new Categoria
            {
                ID = 4,
                Nombre = "Gaming",
                Descripcion = "Periféricos, monitores y componentes para juegos.",
                Imagen = "https://images.unsplash.com/photo-1604846887565-640d2f52d564?q=80&w=1631&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Activa = true
            }
        };

        public IActionResult Index()
        {
            return View(categorias);
        }
    }
}