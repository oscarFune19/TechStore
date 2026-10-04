using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly TechStoreDbContext _context;

        public CategoriaService(TechStoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categoria>> ObtenerTodasAsync()
        {
            return await _context.Categorias.AsNoTracking().ToListAsync();
        }

        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task AgregarAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EditarAsync(Categoria categoria)
        {
            var existente = await _context.Categorias.FindAsync(categoria.ID);
            if (existente == null) return false;

            existente.Nombre = categoria.Nombre;
            existente.Descripcion = categoria.Descripcion;
            existente.Imagen = categoria.Imagen;
            existente.Activa = categoria.Activa;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TieneProductosAsync(int id)
        {
            return await _context.Productos.AnyAsync(p => p.CategoriaId == id);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}