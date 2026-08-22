using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Descripcion = "Laptop moderna ideal para estudio, trabajo y entretenimiento con procesador de alta eficiencia.",
                Precio = 799.99m,
                Categoria = "Computadoras",
                Imagen = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&w=600&q=80",
                Stock = 8,
                Estado = true
            },

            new Producto
            {
                ID = 2,
                Nombre = "Samsung Galaxy S24",
                Descripcion = "Smartphone de alto rendimiento con pantalla Dynamic AMOLED y cámara profesional con IA.",
                Precio = 699.99m,
                Categoria = "Smartphones",
                Imagen = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=600&q=80",
                Stock = 12,
                Estado = true
            },

            new Producto
            {
                ID = 3,
                Nombre = "Audífonos Sony WH-1000XM5",
                Descripcion = "Audífonos inalámbricos con cancelación activa de ruido líder en la industria y sonido Hi-Res.",
                Precio = 349.99m,
                Categoria = "Audio",
                Imagen = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=600&q=80",
                Stock = 6,
                Estado = true
            },

            new Producto
            {
                ID = 4,
                Nombre = "Monitor LG UltraGear",
                Descripcion = "Monitor gaming de 144Hz con panel IPS y tiempo de respuesta de 1ms para juegos competitivos.",
                Precio = 299.99m,
                Categoria = "Gaming",
                Imagen = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=600&q=80",
                Stock = 5,
                Estado = true
            },

            new Producto
            {
                ID = 5,
                Nombre = "Teclado Logitech G915",
                Descripcion = "Teclado mecánico inalámbrico de bajo perfil con iluminación RGB y switches de alta precisión.",
                Precio = 189.99m,
                Categoria = "Gaming",
                Imagen = "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=600&q=80",
                Stock = 0,
                Estado = false
            },

            new Producto
            {
                ID = 6,
                Nombre = "Xiaomi Smart Band",
                Descripcion = "Monitores de ritmo cardíaco, seguimiento de sueño y modos deportivos en pantalla AMOLED.",
                Precio = 59.99m,
                Categoria = "Wearables",
                Imagen = "https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?auto=format&fit=crop&w=600&q=80",
                Stock = 15,
                Estado = true
            }
        };

        public IActionResult Index(string? categoria)
        {
            var resultado = productos.AsEnumerable();

            if (!string.IsNullOrEmpty(categoria))
            {
                resultado = resultado.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
                ViewData["CategoriaSeleccionada"] = categoria;
            }

            return View(resultado.ToList());
        }
    }
}