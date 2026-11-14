using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class ProductoServiceTests
{
    [Fact]
    public async Task ObtenerTodosAsync_ConProductos_RetornaLista()
    {
        await using var context = CrearContexto();

        var categoria = CrearCategoria();
        context.Categorias.Add(categoria);
        context.Productos.AddRange(
            CrearProducto(1, "Laptop", categoria.ID),
            CrearProducto(2, "Mouse", categoria.ID));
        await context.SaveChangesAsync();

        var service = new ProductoService(context);

        var resultado = await service.ObtenerTodosAsync();

        Assert.Equal(2, resultado.Count);
        Assert.NotNull(resultado[0].CategoriaEntidad);
        Assert.Equal("Tecnología", resultado[0].CategoriaEntidad!.Nombre);
    }

    [Fact]
    public async Task ObtenerTodosAsync_SinProductos_RetornaListaVacia()
    {
        await using var context = CrearContexto();
        var service = new ProductoService(context);

        var resultado = await service.ObtenerTodosAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ProductoExiste_RetornaProducto()
    {
        await using var context = CrearContexto();

        var categoria = CrearCategoria();
        context.Categorias.Add(categoria);
        context.Productos.Add(CrearProducto(10, "Teclado", categoria.ID));
        await context.SaveChangesAsync();

        var service = new ProductoService(context);

        var resultado = await service.ObtenerPorIdAsync(10);

        Assert.NotNull(resultado);
        Assert.Equal("Teclado", resultado!.Nombre);
        Assert.NotNull(resultado.CategoriaEntidad);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ProductoNoExiste_RetornaNull()
    {
        await using var context = CrearContexto();
        var service = new ProductoService(context);

        var resultado = await service.ObtenerPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AgregarAsync_ProductoValido_GuardaRegistro()
    {
        await using var context = CrearContexto();

        var categoria = CrearCategoria();
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        var service = new ProductoService(context);
        var producto = CrearProducto(20, "Monitor", categoria.ID);

        await service.AgregarAsync(producto);

        var guardado = await context.Productos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.ID == 20);

        Assert.NotNull(guardado);
        Assert.Equal("Monitor", guardado!.Nombre);
        Assert.True(guardado.Estado);
    }

    [Fact]
    public async Task EditarAsync_ProductoExiste_ActualizaYRetornaTrue()
    {
        await using var context = CrearContexto();

        var categoria = CrearCategoria();
        context.Categorias.Add(categoria);
        context.Productos.Add(CrearProducto(30, "Monitor 24", categoria.ID));
        await context.SaveChangesAsync();

        var service = new ProductoService(context);
        var actualizado = CrearProducto(30, "Monitor 27", categoria.ID);
        actualizado.Precio = 299.99m;
        actualizado.Stock = 7;

        var resultado = await service.EditarAsync(actualizado);

        var guardado = await context.Productos.AsNoTracking()
            .FirstAsync(p => p.ID == 30);

        Assert.True(resultado);
        Assert.Equal("Monitor 27", guardado.Nombre);
        Assert.Equal(299.99m, guardado.Precio);
        Assert.Equal(7, guardado.Stock);
    }

    [Fact]
    public async Task EditarAsync_ProductoNoExiste_RetornaFalse()
    {
        await using var context = CrearContexto();
        var service = new ProductoService(context);

        var resultado = await service.EditarAsync(
            CrearProducto(404, "No existe", 1));

        Assert.False(resultado);
    }

    [Fact]
    public async Task EliminarAsync_ProductoExiste_EliminaYRetornaTrue()
    {
        await using var context = CrearContexto();

        var categoria = CrearCategoria();
        context.Categorias.Add(categoria);
        context.Productos.Add(CrearProducto(40, "Audífonos", categoria.ID));
        await context.SaveChangesAsync();

        var service = new ProductoService(context);

        var resultado = await service.EliminarAsync(40);

        Assert.True(resultado);
        Assert.Empty(await context.Productos.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task EliminarAsync_ProductoNoExiste_RetornaFalse()
    {
        await using var context = CrearContexto();
        var service = new ProductoService(context);

        var resultado = await service.EliminarAsync(999);

        Assert.False(resultado);
    }

    private static TechStoreDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<TechStoreDbContext>()
            .UseInMemoryDatabase($"TechStoreUnitTests-{Guid.NewGuid()}")
            .Options;

        return new TechStoreDbContext(options);
    }

    private static Categoria CrearCategoria(int id = 1)
    {
        return new Categoria
        {
            ID = id,
            Nombre = "Tecnología",
            Descripcion = "Categoría para pruebas",
            Imagen = "categoria.jpg",
            Activa = true
        };
    }

    private static Producto CrearProducto(
        int id,
        string nombre,
        int categoriaId)
    {
        return new Producto
        {
            ID = id,
            Nombre = nombre,
            Descripcion = $"Descripción de {nombre}",
            Precio = 100m,
            CategoriaId = categoriaId,
            Imagen = "producto.jpg",
            Stock = 10,
            Estado = true
        };
    }
}
