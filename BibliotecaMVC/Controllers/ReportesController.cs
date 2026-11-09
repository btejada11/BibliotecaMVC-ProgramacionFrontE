using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private const string ContentTypeExcel =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly BibliotecaContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(BibliotecaContext context, ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Vista de reportería: filtros + vista previa + botones de descarga.
        public async Task<IActionResult> Index(string? autor, string? categoria)
        {
            _logger.LogInformation("Consultando libros para la vista de reportes. Autor: {Autor}, Categoría: {Categoria}.", autor, categoria);
            try
            {
                var libros = await ObtenerLibrosAsync(autor, categoria);

                ViewBag.Autores = await _context.Libros.AsNoTracking()
                    .Where(l => l.Autor != null && l.Autor != "")
                    .Select(l => l.Autor)
                    .Distinct().OrderBy(a => a)
                    .ToListAsync();

                ViewBag.Categorias = await _context.Libros.AsNoTracking()
                    .Where(l => l.Categoria != null && l.Categoria != "")
                    .Select(l => l.Categoria)
                    .Distinct().OrderBy(c => c)
                    .ToListAsync();

                ViewBag.AutorSeleccionado = autor;
                ViewBag.CategoriaSeleccionada = categoria;

                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la vista de reportes de libros.");
                throw;
            }
        }

        // Generar reporte PDF (QuestPDF)
        public async Task<IActionResult> DescargarPdf(string? autor, string? categoria)
        {
            _logger.LogInformation("Generando reporte PDF de libros. Autor: {Autor}, Categoría: {Categoria}.", autor, categoria);
            try
            {
                var libros = await ObtenerLibrosAsync(autor, categoria);
                var pdf = ReporteLibrosPdf.Generar(libros, autor, categoria, DateTime.Now);
                return File(pdf, "application/pdf", "ReporteLibros.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte PDF de libros.");
                throw;
            }
        }

        // Exportar a Excel (ClosedXML)
        public async Task<IActionResult> DescargarExcel(string? autor, string? categoria)
        {
            _logger.LogInformation("Generando reporte Excel de libros. Autor: {Autor}, Categoría: {Categoria}.", autor, categoria);
            try
            {
                var libros = await ObtenerLibrosAsync(autor, categoria);
                var excel = ReporteLibrosExcel.Generar(libros, autor, categoria, DateTime.Now);
                return File(excel, ContentTypeExcel, "ReporteLibros.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el reporte Excel de libros.");
                throw;
            }
        }

        // Misma consulta (EF Core) para la vista, el PDF y el Excel:
        // así los tres siempre muestran exactamente los mismos datos.
        private async Task<List<Libro>> ObtenerLibrosAsync(string? autor, string? categoria)
        {
            var query = _context.Libros.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(autor))
                query = query.Where(l => l.Autor == autor);

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(l => l.Categoria == categoria);

            return await query.OrderBy(l => l.ID).ToListAsync();
        }
    }
}