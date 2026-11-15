using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TechStore.Models;
using TechStore.Services;

namespace TechStore.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService,
                                   ICategoriaService categoriaService,
                                   ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _logger = logger;
        }

        // LISTADO
        public async Task<IActionResult> Index(string? categoria)
        {
            _logger.LogInformation("Iniciando consulta de productos");
            try
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

                _logger.LogInformation("Consulta de productos completada");
                return View(productos);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al consultar los productos");
                throw;
            }
        }

        // DETALLE
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Iniciando consulta del producto con identificador {ProductoId}", id);
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                if (producto == null)
                {
                    _logger.LogWarning("No se encontró el producto con identificador {ProductoId}", id);
                    return NotFound();
                }
                return View(producto);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al consultar el producto con identificador {ProductoId}", id);
                throw;
            }
        }

        // REPORTES
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Reporte()
        {
            var productos = (await _productoService.ObtenerTodosAsync())
                .OrderBy(producto => producto.Nombre)
                .ToList();

            return File(GenerarReporte(productos), "application/pdf");
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> DescargarReporte()
        {
            var productos = (await _productoService.ObtenerTodosAsync())
                .OrderBy(producto => producto.Nombre)
                .ToList();

            return File(GenerarReporte(productos), "application/pdf", "Reporte_Productos.pdf");
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> ExportarExcel()
        {
            var productos = (await _productoService.ObtenerTodosAsync())
                .OrderBy(producto => producto.Nombre)
                .ToList();

            using var libroExcel = new XLWorkbook();
            var hoja = libroExcel.Worksheets.Add("Reporte de Productos");

            hoja.Cell("A1").Value = "Reporte de Productos";
            hoja.Range("A1:F1").Merge();
            hoja.Cell("A1").Style.Font.Bold = true;
            hoja.Cell("A1").Style.Font.FontSize = 16;
            hoja.Cell("A1").Style.Font.FontColor = XLColor.White;
            hoja.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            hoja.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            hoja.Row(1).Height = 28;

            hoja.Cell("A2").Value = "Fecha de generación:";
            hoja.Cell("B2").Value = DateTime.Now;
            hoja.Cell("B2").Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            hoja.Range("A2:F2").Style.Font.Italic = true;

            var encabezados = new[] { "ID", "Nombre del producto", "Categoría", "Precio", "Stock", "Estado" };
            for (var columna = 0; columna < encabezados.Length; columna++)
            {
                var celda = hoja.Cell(4, columna + 1);
                celda.Value = encabezados[columna];
                celda.Style.Font.Bold = true;
                celda.Style.Font.FontColor = XLColor.White;
                celda.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                celda.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                celda.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            for (var fila = 0; fila < productos.Count; fila++)
            {
                var producto = productos[fila];
                var numeroFila = fila + 5;

                hoja.Cell(numeroFila, 1).Value = producto.ID;
                hoja.Cell(numeroFila, 2).Value = producto.Nombre;
                hoja.Cell(numeroFila, 3).Value = producto.CategoriaEntidad?.Nombre ?? "No especificada";
                hoja.Cell(numeroFila, 4).Value = producto.Precio;
                hoja.Cell(numeroFila, 5).Value = producto.Stock;
                hoja.Cell(numeroFila, 6).Value = producto.Estado ? "Activo" : "Inactivo";
            }

            var rangoReporte = hoja.Range(4, 1, Math.Max(4, productos.Count + 4), 6);
            rangoReporte.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoReporte.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rangoReporte.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B4C7E7");
            rangoReporte.Style.Border.InsideBorderColor = XLColor.FromHtml("#D9E2F3");
            rangoReporte.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            hoja.Range(5, 1, Math.Max(5, productos.Count + 4), 1)
                .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Range(4, 1, Math.Max(4, productos.Count + 4), 6).SetAutoFilter();
            hoja.SheetView.FreezeRows(4);
            hoja.Columns(1, 6).AdjustToContents();
            hoja.Column(1).Width = Math.Max(hoja.Column(1).Width, 8);
            hoja.Column(2).Width = Math.Min(Math.Max(hoja.Column(2).Width, 20), 45);
            hoja.Column(3).Width = Math.Min(Math.Max(hoja.Column(3).Width, 18), 35);
            hoja.Column(4).Width = Math.Max(hoja.Column(4).Width, 12);

            using var memoria = new MemoryStream();
            libroExcel.SaveAs(memoria);

            return File(
                memoria.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Reporte_Productos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private static byte[] GenerarReporte(IReadOnlyCollection<Producto> productos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document => document.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(42);
                page.PageColor(Colors.Grey.Lighten5);
                page.Header().Column(column =>
                {
                    column.Item().Text("TECHSTORE")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Reporte de productos")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.Grey.Darken3);
                    column.Item().Text($"Listado actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(14).LineHorizontal(2).LineColor(Colors.Blue.Medium);
                });
                page.Content().PaddingVertical(22).Column(column =>
                {
                    column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>
                    {
                        row.RelativeItem().Text("Productos Registrados")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(productos.Count.ToString())
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.White);
                    });

                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1.3f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("Nombre");
                            header.Cell().Element(HeaderCellStyle).Text("Categoria");
                            header.Cell().Element(HeaderCellStyle).Text("Precio");
                            header.Cell().Element(HeaderCellStyle).Text("Stock");
                            header.Cell().Element(HeaderCellStyle).Text("Estado");
                        });

                        foreach (var producto in productos)
                        {
                            table.Cell().Element(BodyCellStyle).Text(producto.ID.ToString());
                            table.Cell().Element(BodyCellStyle).Text(producto.Nombre);
                            table.Cell().Element(BodyCellStyle).Text(producto.CategoriaEntidad?.Nombre ?? "No especificada");
                            table.Cell().Element(BodyCellStyle).Text($"${producto.Precio:N2}");
                            table.Cell().Element(BodyCellStyle).Text(producto.Stock.ToString());
                            table.Cell().Element(BodyCellStyle).Text(producto.Estado ? "Activo" : "Inactivo");
                        }
                    });
                });
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("TechStore - Pagina ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            })).GeneratePdf();

            static IContainer HeaderCellStyle(IContainer container) => container
           .Background(Colors.Blue.Darken2)
           .PaddingVertical(9)
           .PaddingHorizontal(8)
           .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.White));

            static IContainer BodyCellStyle(IContainer container) => container
           .Background(Colors.White)
           .BorderBottom(1)
           .BorderColor(Colors.Grey.Lighten2)
           .PaddingVertical(9)
           .PaddingHorizontal(8)
           .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.Grey.Darken3));
        }

        // AGREGAR
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Iniciando formulario de creación de producto");
            try
            {
                await CargarCategoriasAsync();
                return View(new Producto { Estado = true });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar el formulario de creación de producto");
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto)
        {
            _logger.LogInformation("Iniciando creación de producto");
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("La creación de producto contiene datos no válidos");
                    await CargarCategoriasAsync(producto.CategoriaId);
                    return View(producto);
                }

                await _productoService.AgregarAsync(producto);
                _logger.LogInformation("Producto creado correctamente con identificador {ProductoId}", producto.ID);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al crear el producto");
                throw;
            }
        }

        // EDITAR
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Iniciando formulario de edición del producto con identificador {ProductoId}", id);
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                if (producto == null)
                {
                    _logger.LogWarning("No se encontró el producto con identificador {ProductoId} para editar", id);
                    return NotFound();
                }

                await CargarCategoriasAsync(producto.CategoriaId);
                return View(producto);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar el producto con identificador {ProductoId} para editar", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            _logger.LogInformation("Iniciando edición del producto con identificador {ProductoId}", id);
            try
            {
                if (id != producto.ID)
                {
                    _logger.LogWarning("El identificador {ProductoId} no coincide con el del producto enviado", id);
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("La edición del producto con identificador {ProductoId} contiene datos no válidos", id);
                    await CargarCategoriasAsync(producto.CategoriaId);
                    return View(producto);
                }

                var editado = await _productoService.EditarAsync(producto);
                if (!editado)
                {
                    _logger.LogWarning("No se encontró el producto con identificador {ProductoId} para editar", id);
                    return NotFound();
                }

                _logger.LogInformation("Producto actualizado correctamente con identificador {ProductoId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al editar el producto con identificador {ProductoId}", id);
                throw;
            }
        }

        // ELIMINAR (pantalla de confirmación)
        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Iniciando formulario de eliminación del producto con identificador {ProductoId}", id);
            try
            {
                var producto = await _productoService.ObtenerPorIdAsync(id);
                if (producto == null)
                {
                    _logger.LogWarning("No se encontró el producto con identificador {ProductoId} para eliminar", id);
                    return NotFound();
                }

                return View(producto);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar el producto con identificador {ProductoId} para eliminar", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Iniciando eliminación del producto con identificador {ProductoId}", id);
            try
            {
                var eliminado = await _productoService.EliminarAsync(id);
                if (!eliminado)
                {
                    _logger.LogWarning("No se encontró el producto con identificador {ProductoId} para eliminar", id);
                    return NotFound();
                }

                _logger.LogInformation("Producto eliminado correctamente con identificador {ProductoId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al eliminar el producto con identificador {ProductoId}", id);
                throw;
            }
        }

        // Llena el selector de categorías
        private async Task CargarCategoriasAsync(int? seleccionada = null)
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            ViewBag.Categorias = new SelectList(categorias, "ID", "Nombre", seleccionada);
        }
    }
}
