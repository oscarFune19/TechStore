using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TechStore.Controllers;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Tests;

public class ProductosControllerTests
{
    private readonly Mock<IProductoService> _productoServiceMock = new();
    private readonly Mock<ICategoriaService> _categoriaServiceMock = new();

    [Fact]
    public async Task Index_ConProductos_RetornaVistaConModelo()
    {
        var productos = new List<Producto>
        {
            CrearProducto(1, "Laptop"),
            CrearProducto(2, "Mouse")
        };

        _productoServiceMock
            .Setup(s => s.ObtenerTodosAsync())
            .ReturnsAsync(productos);

        var controller = CrearController();

        var resultado = await controller.Index(null);

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<List<Producto>>(vista.Model);

        Assert.Equal(2, modelo.Count);
        _productoServiceMock.Verify(
            s => s.ObtenerTodosAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Create_Post_ProductoValido_AgregaYRedirige()
    {
        var producto = CrearProducto(3, "Monitor");

        _productoServiceMock
            .Setup(s => s.AgregarAsync(producto))
            .Returns(Task.CompletedTask);

        var controller = CrearController();

        var resultado = await controller.Create(producto);

        var redirect = Assert.IsType<RedirectToActionResult>(resultado);

        Assert.Equal(nameof(ProductosController.Index), redirect.ActionName);
        _productoServiceMock.Verify(
            s => s.AgregarAsync(producto),
            Times.Once);
    }

    [Fact]
    public async Task Create_Post_ModeloInvalido_NoAgregaProducto()
    {
        var producto = CrearProducto(4, string.Empty);

        _categoriaServiceMock
            .Setup(s => s.ObtenerTodasAsync())
            .ReturnsAsync(new List<Categoria>());

        var controller = CrearController();
        controller.ModelState.AddModelError(
            nameof(Producto.Nombre),
            "El nombre es requerido");

        var resultado = await controller.Create(producto);

        Assert.IsType<ViewResult>(resultado);
        Assert.False(controller.ModelState.IsValid);
        _productoServiceMock.Verify(
            s => s.AgregarAsync(It.IsAny<Producto>()),
            Times.Never);
    }

    [Fact]
    public async Task Edit_Post_ProductoExiste_ActualizaYRedirige()
    {
        var producto = CrearProducto(5, "Teclado");

        _productoServiceMock
            .Setup(s => s.EditarAsync(producto))
            .ReturnsAsync(true);

        var controller = CrearController();

        var resultado = await controller.Edit(producto.ID, producto);

        var redirect = Assert.IsType<RedirectToActionResult>(resultado);

        Assert.Equal(nameof(ProductosController.Index), redirect.ActionName);
        _productoServiceMock.Verify(
            s => s.EditarAsync(producto),
            Times.Once);
    }

    [Fact]
    public async Task Edit_Post_ProductoNoExiste_RetornaNotFound()
    {
        var producto = CrearProducto(500, "No existe");

        _productoServiceMock
            .Setup(s => s.EditarAsync(producto))
            .ReturnsAsync(false);

        var controller = CrearController();

        var resultado = await controller.Edit(producto.ID, producto);

        Assert.IsType<NotFoundResult>(resultado);
        _productoServiceMock.Verify(
            s => s.EditarAsync(producto),
            Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ProductoExiste_EliminaYRedirige()
    {
        _productoServiceMock
            .Setup(s => s.EliminarAsync(6))
            .ReturnsAsync(true);

        var controller = CrearController();

        var resultado = await controller.DeleteConfirmed(6);

        var redirect = Assert.IsType<RedirectToActionResult>(resultado);

        Assert.Equal(nameof(ProductosController.Index), redirect.ActionName);
        _productoServiceMock.Verify(
            s => s.EliminarAsync(6),
            Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_ProductoNoExiste_RetornaNotFound()
    {
        _productoServiceMock
            .Setup(s => s.EliminarAsync(999))
            .ReturnsAsync(false);

        var controller = CrearController();

        var resultado = await controller.DeleteConfirmed(999);

        Assert.IsType<NotFoundResult>(resultado);
        _productoServiceMock.Verify(
            s => s.EliminarAsync(999),
            Times.Once);
    }

    [Fact]
    public async Task Details_ProductoExiste_RetornaVistaConElProducto()
    {
        var producto = CrearProducto(1, "Laptop");

        _productoServiceMock
            .Setup(s => s.ObtenerPorIdAsync(1))
            .ReturnsAsync(producto);

        var controller = CrearController();

        var resultado = await controller.Details(1);

        var vista = Assert.IsType<ViewResult>(resultado);
        Assert.Same(producto, vista.Model);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task Details_ProductoNoExiste_RetornaNotFound(int id)
    {
        _productoServiceMock
            .Setup(s => s.ObtenerPorIdAsync(id))
            .ReturnsAsync((Producto?)null);

        var controller = CrearController();

        var resultado = await controller.Details(id);

        Assert.IsType<NotFoundResult>(resultado);
    }

    private ProductosController CrearController()
    {
        return new ProductosController(
            _productoServiceMock.Object,
            _categoriaServiceMock.Object,
            NullLogger<ProductosController>.Instance);
    }

    private static Producto CrearProducto(int id, string nombre)
    {
        return new Producto
        {
            ID = id,
            Nombre = nombre,
            Descripcion = $"Descripción de {nombre}",
            Precio = 100m,
            CategoriaId = 1,
            Imagen = "producto.jpg",
            Stock = 10,
            Estado = true
        };
    }
}
