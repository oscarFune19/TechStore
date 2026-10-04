using TechStore.Models;

namespace TechStore.Services
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> ObtenerTodasAsync();
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Categoria categoria);
        Task<bool> EditarAsync(Categoria categoria);
        Task<bool> TieneProductosAsync(int id);
        Task<bool> EliminarAsync(int id);
    }
}