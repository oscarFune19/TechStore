using TechStore.Models;

namespace TechStore.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Producto producto);
        Task<bool> EditarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
    }
}
