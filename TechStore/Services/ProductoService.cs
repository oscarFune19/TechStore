using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Services
{
    public class ProductoService : IProductoService
    {
        private readonly TechStoreDbContext _context;

        public ProductoService(TechStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Include(p => p.CategoriaEntidad)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.CategoriaEntidad)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        public async Task AgregarAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EditarAsync(Producto producto)
        {
            var productoExistente = await _context.Productos.FindAsync(producto.ID);

            if (productoExistente == null)
                return false;

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Descripcion = producto.Descripcion;
            productoExistente.Precio = producto.Precio;
            productoExistente.CategoriaId = producto.CategoriaId;
            productoExistente.Imagen = producto.Imagen;
            productoExistente.Stock = producto.Stock;
            productoExistente.Estado = producto.Estado;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
