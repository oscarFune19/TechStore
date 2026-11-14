using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;

        public ProductosController(IProductoService productoService,
                                   ICategoriaService categoriaService)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
        }

        // LISTADO
        public async Task<IActionResult> Index(string? categoria)
        {
            var productos = await _productoService.ObtenerTodosAsync();

            if (!string.IsNullOrEmpty(categoria))
            {
                productos = productos
                    .Where(p => p.CategoriaEntidad != null &&
                                p.CategoriaEntidad.Nombre.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                ViewData["CategoriaSeleccionada"] = categoria;
            }

            return View(productos);
        }

        // AGREGAR
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Create()
        {
            await CargarCategoriasAsync();
            return View(new Producto { Estado = true });
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(producto.CategoriaId);
                return View(producto);
            }

            await _productoService.AgregarAsync(producto);
            return RedirectToAction(nameof(Index));
        }

        // EDITAR
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();

            await CargarCategoriasAsync(producto.CategoriaId);
            return View(producto);
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.ID) return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(producto.CategoriaId);
                return View(producto);
            }

            var editado = await _productoService.EditarAsync(producto);
            if (!editado) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR (pantalla de confirmación)
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eliminado = await _productoService.EliminarAsync(id);
            if (!eliminado) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // Llena el selector de categorías
        private async Task CargarCategoriasAsync(int? seleccionada = null)
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            ViewBag.Categorias = new SelectList(categorias, "ID", "Nombre", seleccionada);
        }
    }
}