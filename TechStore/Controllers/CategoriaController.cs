using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string? _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("TechStoreConnection");
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando consulta de categorías");
            try
            {
                var categorias = ObtenerCategorias();
                _logger.LogInformation("Consulta de categorías completada");
                return View(categorias);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al consultar las categorías");
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public IActionResult Reporte()
        {
            return File(GenerarReporte(ObtenerCategorias()), "application/pdf");
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public IActionResult DescargarReporte()
        {
            return File(GenerarReporte(ObtenerCategorias()), "application/pdf", "Reporte_Categorias.pdf");
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando formulario de creación de categoría");
            return View(new Categoria());
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation("Iniciando creación de categoría");
            if (categoria == null || !ModelState.IsValid)
            {
                _logger.LogWarning("La creación de categoría contiene datos no válidos");
                return View(categoria);
            }
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Categorias (Nombre, Descripcion, Imagen, Activa) VALUES (@Nombre, @Descripcion, @Imagen, @Activa)";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", categoria.Descripcion);
                        comando.Parameters.AddWithValue("@Imagen", categoria.Imagen);
                        comando.Parameters.AddWithValue("@Activa", categoria.Activa);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
                _logger.LogInformation("Categoría creada correctamente");
                TempData["SuccessMessage"] = "Categoria creada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al crear la categoría");
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando formulario de edición de la categoría con identificador {CategoriaId}", id);
            try
            {
                var categoria = ObtenerCategoriaPorId(id);
                if (categoria == null)
                {
                    _logger.LogWarning("No se encontró la categoría con identificador {CategoriaId} para editar", id);
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar la categoría con identificador {CategoriaId} para editar", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            _logger.LogInformation("Iniciando edición de la categoría con identificador {CategoriaId}", categoria?.ID);
            if (categoria == null || !ModelState.IsValid)
            {
                _logger.LogWarning("La edición de categoría contiene datos no válidos");
                return View(categoria);
            }
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion, Imagen = @Imagen, Activa = @Activa WHERE ID = @ID";

                    using (var cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", categoria.Descripcion);
                        cmd.Parameters.AddWithValue("@Imagen", categoria.Imagen);
                        cmd.Parameters.AddWithValue("@Activa", categoria.Activa);
                        cmd.Parameters.AddWithValue("@ID", categoria.ID);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                _logger.LogInformation("Categoría actualizada correctamente con identificador {CategoriaId}", categoria.ID);
                TempData["SuccessMessage"] = "Categoria actualizada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al editar la categoría con identificador {CategoriaId}", categoria.ID);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando formulario de eliminación de la categoría con identificador {CategoriaId}", id);
            try
            {
                var categoria = ObtenerCategoriaPorId(id);
                if (categoria == null)
                {
                    _logger.LogWarning("No se encontró la categoría con identificador {CategoriaId} para eliminar", id);
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar la categoría con identificador {CategoriaId} para eliminar", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Iniciando eliminación de la categoría con identificador {CategoriaId}", id);
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Categorias WHERE ID = @ID";

                    using (var cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);

                        conexion.Open();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            _logger.LogWarning("No se encontró la categoría con identificador {CategoriaId} para eliminar", id);
                            return NotFound();
                        }
                    }
                }
                _logger.LogInformation("Categoría eliminada correctamente con identificador {CategoriaId}", id);
                TempData["SuccessMessage"] = "Categoria eliminada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al eliminar la categoría con identificador {CategoriaId}", id);
                throw;
            }
        }

        // Consultas a la base de datos (SqlClient)
        private List<Categoria> ObtenerCategorias()
        {
            var categorias = new List<Categoria>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var SQL = "SELECT ID, Nombre, Descripcion, Imagen, Activa FROM Categorias ORDER BY Nombre";
                using (var comando = new SqlCommand(SQL, connection))
                {
                    connection.Open();
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                ID = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Descripcion = lector.GetString(2),
                                Imagen = lector.GetString(3),
                                Activa = lector.GetBoolean(4)
                            });
                        }
                    }
                }
            }
            return categorias;
        }

        private Categoria? ObtenerCategoriaPorId(int id)
        {
            Categoria? categoria = null;
            using (var conexion = new SqlConnection(_connectionString))
            {
                var sql = "SELECT ID, Nombre, Descripcion, Imagen, Activa FROM Categorias WHERE ID = @ID";

                using (var cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    conexion.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            categoria = new Categoria
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Nombre = reader["Nombre"].ToString() ?? string.Empty,
                                Descripcion = reader["Descripcion"].ToString() ?? string.Empty,
                                Imagen = reader["Imagen"].ToString() ?? string.Empty,
                                Activa = Convert.ToBoolean(reader["Activa"])
                            };
                        }
                    }
                }
            }
            return categoria;
        }

        private static byte[] GenerarReporte(IReadOnlyCollection<Categoria> categorias)
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
                    column.Item().Text("Reporte de categorías")
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
                        row.RelativeItem().Text("Categorías Registradas")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(categorias.Count.ToString())
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.White);
                    });

                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(3f);
                            columns.RelativeColumn(1f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("Nombre");
                            header.Cell().Element(HeaderCellStyle).Text("Descripcion");
                            header.Cell().Element(HeaderCellStyle).Text("Activa");
                        });

                        foreach (var categoria in categorias)
                        {
                            table.Cell().Element(BodyCellStyle).Text(categoria.ID.ToString());
                            table.Cell().Element(BodyCellStyle).Text(categoria.Nombre);
                            table.Cell().Element(BodyCellStyle).Text(categoria.Descripcion);
                            table.Cell().Element(BodyCellStyle).Text(categoria.Activa ? "Sí" : "No");
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
    }
}
