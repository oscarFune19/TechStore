using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class ProductoServiceIntegrationTests
{
    [Fact]
    public async Task FlujoCrud_ConSqliteEnMemoria_FuncionaCorrectamente()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TechStoreDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new TechStoreDbContext(options);

        await context.Database.EnsureCreatedAsync();

        var categoria = new Categoria
        {
            Nombre = "Computadoras",
            Descripcion = "Categoría de integración",
            Imagen = "computadoras.jpg",
            Activa = true
        };

        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        var service = new ProductoService(context);

        var producto = new Producto
        {
            Nombre = "Laptop Integración",
            Descripcion = "Producto creado en prueba de integración",
            Precio = 899.99m,
            CategoriaId = categoria.ID,
            Imagen = "laptop.jpg",
            Stock = 5,
            Estado = true
        };

        await service.AgregarAsync(producto);

        var creado = await service.ObtenerPorIdAsync(producto.ID);

        Assert.NotNull(creado);
        Assert.Equal("Laptop Integración", creado!.Nombre);
        Assert.NotNull(creado.CategoriaEntidad);

        var actualizado = new Producto
        {
            ID = producto.ID,
            Nombre = "Laptop Integración Actualizada",
            Descripcion = "Producto actualizado",
            Precio = 949.99m,
            CategoriaId = categoria.ID,
            Imagen = "laptop-actualizada.jpg",
            Stock = 3,
            Estado = true
        };

        var editado = await service.EditarAsync(actualizado);
        var despuesDeEditar =
            await service.ObtenerPorIdAsync(producto.ID);

        Assert.True(editado);
        Assert.NotNull(despuesDeEditar);
        Assert.Equal(
            "Laptop Integración Actualizada",
            despuesDeEditar!.Nombre);
        Assert.Equal(3, despuesDeEditar.Stock);

        var eliminado = await service.EliminarAsync(producto.ID);
        var despuesDeEliminar =
            await service.ObtenerPorIdAsync(producto.ID);

        Assert.True(eliminado);
        Assert.Null(despuesDeEliminar);
    }
}
