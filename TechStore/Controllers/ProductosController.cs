using Microsoft.AspNetCore.Mvc;
using TechStore.Models;

namespace TechStore.Controllers
{
    public class ProductosController : Controller
    {
        private static readonly List<Producto> productos = new()
        {
            new Producto
            {
                ID = 1,
                Nombre = "Laptop ASUS VivoBook 15",
                Descripcion = "Laptop moderna ideal para estudio, trabajo y entretenimiento.",
                Precio = 799.99m,
                Categoria = "Computadoras",
                Imagen = "/images/laptop-asus.jpg",
                Stock = 8,
                Estado = true
            },

            new Producto
            {
                ID = 2,
                Nombre = "Samsung Galaxy S24",
                Descripcion = "Smartphone de alto rendimiento con excelente cámara y pantalla.",
                Precio = 699.99m,
                Categoria = "Smartphones",
                Imagen = "/images/smartphone-samsung.jpg",
                Stock = 12,
                Estado = true
            },

            new Producto
            {
                ID = 3,
                Nombre = "Audífonos Sony WH-1000XM5",
                Descripcion = "Audífonos inalámbricos con cancelación activa de ruido.",
                Precio = 349.99m,
                Categoria = "Audio",
                Imagen = "/images/audifonos-sony.jpg",
                Stock = 6,
                Estado = true
            },

            new Producto
            {
                ID = 4,
                Nombre = "Monitor LG UltraGear",
                Descripcion = "Monitor gaming de alta frecuencia para una experiencia fluida.",
                Precio = 299.99m,
                Categoria = "Gaming",
                Imagen = "/images/monitor-lg.jpg",
                Stock = 5,
                Estado = true
            },

            new Producto
            {
                ID = 5,
                Nombre = "Teclado Logitech G915",
                Descripcion = "Teclado mecánico inalámbrico para gaming y productividad.",
                Precio = 189.99m,
                Categoria = "Accesorios",
                Imagen = "/images/teclado-logitech.jpg",
                Stock = 0,
                Estado = false
            },

            new Producto
            {
                ID = 6,
                Nombre = "Xiaomi Smart Band",
                Descripcion = "Dispositivo inteligente para actividad física y notificaciones.",
                Precio = 59.99m,
                Categoria = "Wearables",
                Imagen = "/images/smartwatch-xiaomi.jpg",
                Stock = 15,
                Estado = true
            }
        };

        public IActionResult Index()
        {
            return View(productos);
        }
    }
}