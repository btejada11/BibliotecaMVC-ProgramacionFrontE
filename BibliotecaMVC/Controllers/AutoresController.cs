using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;
        private readonly ILogger<AutoresController> _logger;

        public AutoresController(IAutorService autorService, ILogger<AutoresController> logger)
        {
            _autorService = autorService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando operación de consulta: Obtener listado completo de autores.");
            try
            {
                var autores = _autorService.ObtenerTodos();
                return View(autores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al intentar obtener el listado de autores.");
                throw;
            }
        }

        public IActionResult Reportes()
        {
            _logger.LogInformation("Iniciando generación del reporte PDF de autores.");
            try
            {
                var autores = _autorService
                    .ObtenerTodos()
                    .OrderBy(a => a.Nombre)
                    .ToList();

                var pdf = GenerarReportes(autores);
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte de autores.");
                throw;
            }
        }

        public IActionResult DescargarReportes()
        {
            _logger.LogInformation("Iniciando generación del reporte PDF de autores para descarga.");
            try
            {
                var autores = _autorService
                    .ObtenerTodos()
                    .OrderBy(a => a.Nombre)
                    .ToList();

                var pdf = GenerarReportes(autores);
                return File(pdf, "application/pdf", "ReporteAutores.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte de autores para descarga.");
                throw;
            }
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten4)
                .PaddingVertical(5)
                .PaddingHorizontal(4);
        }

        private static byte[] GenerarReportes(IReadOnlyCollection<Autor> autores)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Fecha y hora en que se genera/descarga el reporte
            var fechaGeneracion = DateTime.Now;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("BibliotecaMVC - Reporte de Autores")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50); // ID
                            columns.RelativeColumn();   // Nombre
                            columns.RelativeColumn();   // Apellido
                            columns.RelativeColumn();   // Nacionalidad
                            columns.RelativeColumn();   // Fecha de Nacimiento
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID").SemiBold();
                            header.Cell().Element(CellStyle).Text("Nombre").SemiBold();
                            header.Cell().Element(CellStyle).Text("Apellido").SemiBold();
                            header.Cell().Element(CellStyle).Text("Nacionalidad").SemiBold();
                            header.Cell().Element(CellStyle).Text("Fecha de Nacimiento").SemiBold();
                        });

                        foreach (var autor in autores)
                        {
                            table.Cell().Element(CellStyle).Text(autor.Id.ToString());
                            table.Cell().Element(CellStyle).Text(autor.Nombre);
                            table.Cell().Element(CellStyle).Text(autor.Apellido);
                            table.Cell().Element(CellStyle).Text(autor.Nacionalidad);
                            table.Cell().Element(CellStyle).Text(autor.FechaNacimiento.ToShortDateString());
                        }
                    });

                    page.Footer().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"Generado el {fechaGeneracion:dd/MM/yyyy HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);

                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken1));
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                });
            }).GeneratePdf();
        }

        public IActionResult Details(int id)
        {
            _logger.LogInformation("Iniciando operación de consulta: Buscando detalles del autor con ID {id}.", id);
            try
            {
                var autor = _autorService.ObtenerAutorPorId(id);
                if (autor == null)
                {
                    _logger.LogWarning("Situación inesperada: No se encontró el autor con ID {id} durante la consulta de detalles.", id);
                    return NotFound("Autor no encontrado");
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar los detalles del autor con ID {id}.", id);
                throw;
            }
        }

        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando flujo de edición (GET): Recuperando autor con ID {id}.", id);
            try
            {
                var autor = _autorService.ObtenerAutorPorId(id);
                if (autor == null)
                {
                    _logger.LogWarning("Situación inesperada: No se encontró el autor con ID {id} para cargar el formulario de edición.", id);
                    return NotFound("Autor no encontrado");
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar los datos del autor con ID {id} para su edición.", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Edit(Autor autor)
        {
            _logger.LogInformation("Iniciando operación de edición (POST): Procesando actualización para el autor con ID {id}.", autor.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo enviado para la edición del autor con ID {id} no es válido.", autor.Id);
                return View(autor);
            }

            try
            {
                var autorExistente = _autorService.ObtenerAutorPorId(autor.Id);
                if (autorExistente == null)
                {
                    _logger.LogWarning("Situación inesperada: Intento de actualizar un autor inexistente con ID {id}.", autor.Id);
                    return NotFound("Autor no encontrado");
                }

                _autorService.Actualizar(autor);
                _logger.LogInformation("Operación exitosa: Autor con ID {id} actualizado correctamente.", autor.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al persistir los cambios del autor con ID {id} en la base de datos.", autor.Id);
                throw;
            }
        }

        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Accediendo al formulario de creación de autores.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Create(Autor autor)
        {
            _logger.LogInformation("Iniciando operación de creación: Registrando un nuevo autor.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo enviado para la creación del autor no es válido.");
                return View(autor);
            }

            try
            {
                autor.Activo = true;
                _autorService.Agregar(autor);
                _logger.LogInformation("Operación exitosa: Nuevo autor registrado con éxito.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar registrar el nuevo autor en el sistema.");
                throw;
            }
        }

        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando flujo de eliminación: Confirmando eliminación del autor con ID {id}.", id);
            try
            {
                var autor = _autorService.ObtenerAutorPorId(id);
                if (autor == null)
                {
                    _logger.LogWarning("Situación inesperada: No se encontró el autor con ID {id} para la confirmación de eliminación.", id);
                    return NotFound("Autor no encontrado");
                }
                return View(autor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar el autor con ID {id} antes de su eliminación.", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult DeleteDeVelda(int id)
        {
            _logger.LogInformation("Iniciando operación de eliminación: Confirmando baja del autor con ID {id}.", id);

            try
            {
                var autorExistente = _autorService.ObtenerAutorPorId(id);
                if (autorExistente == null)
                {
                    _logger.LogWarning("Situación inesperada: Intento de eliminar un autor inexistente con ID {id}.", id);
                    return NotFound("Autor no encontrado");
                }

                _autorService.Eliminar(id);
                _logger.LogInformation("Operación exitosa: Autor con ID {id} eliminado de manera lógica o física.", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al procesar la eliminación del autor con ID {id}.", id);
                throw;
            }
        }
    }
}