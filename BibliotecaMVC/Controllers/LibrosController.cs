using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;

        // Se inyecta ILogger preservando la dependencia existente del contexto de datos
        public LibrosController(BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando consulta: Recuperando el listado completo de libros.");
            try
            {
                var libros = _context.Libros.ToList();
                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al intentar obtener el listado de libros desde la base de datos.");
                throw;
            }
        }

        public IActionResult Details(int id)
        {
            _logger.LogInformation("Iniciando consulta: Buscando detalles del libro con ID {id}.", id);
            try
            {
                var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
                if (libro == null)
                {
                    _logger.LogWarning("Situación inesperada: No se encontró el libro con ID {id} durante la consulta de detalles.", id);
                    return NotFound("Libro no encontrado");
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la consulta de detalles para el libro con ID {id}.", id);
                throw;
            }
        }

        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Accediendo al formulario de creación de libros (GET).");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Create(Libro libro)
        {
            _logger.LogInformation("Iniciando operación de creación (POST): Registrando un nuevo libro titulado '{Titulo}'.", libro.Titulo);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo enviado para la creación del libro no es válido debido a errores de validación.");
                return View(libro);
            }

            try
            {
                libro.Disponible = true;
                _context.Libros.Add(libro);
                _context.SaveChanges();

                _logger.LogInformation("Operación exitosa: El libro '{Titulo}' con ID {ID} fue registrado correctamente.", libro.Titulo, libro.ID);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar insertar el nuevo libro en la base de datos.");
                throw;
            }
        }

        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando flujo de edición (GET): Buscando libro con ID {id} para cargar el formulario.", id);
            try
            {
                var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
                if (libro == null)
                {
                    _logger.LogWarning("Situación inesperada: No se encontró el libro con ID {id} para iniciar su edición.", id);
                    return NotFound("Libro no encontrado");
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar el libro con ID {id} para el flujo de edición.", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Edit(Libro libro) // Nota: Asegúrate de mapear al nombre exacto de tu modelo 'Libro'
        {
            _logger.LogInformation("Iniciando operación de edición (POST): Procesando cambios para el libro con ID {id}.", libro.ID);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo enviado para la edición del libro con ID {id} no pasó las validaciones.", libro.ID);
                return View(libro);
            }

            try
            {
                var libroExistente = _context.Libros.FirstOrDefault(l => l.ID == libro.ID);
                if (libroExistente == null)
                {
                    _logger.LogWarning("Situación inesperada: Intento fallido de editar un libro inexistente con ID {id}.", libro.ID);
                    return NotFound("Libro no encontrado");
                }

                // Actualización de campos
                libroExistente.Titulo = libro.Titulo;
                libroExistente.Autor = libro.Autor;
                libroExistente.Categoria = libro.Categoria;
                libroExistente.Precio = libro.Precio;
                libroExistente.Disponible = libro.Disponible;

                _context.SaveChanges();
                _logger.LogInformation("Operación exitosa: Cambios guardados para el libro con ID {id}.", libro.ID);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Error de concurrencia al intentar guardar los cambios del libro con ID {id}.", libro.ID);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al actualizar el libro con ID {id} en la base de datos.", libro.ID);
                throw;
            }
        }

        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando flujo de eliminación (GET): Solicitando confirmación de borrado para el libro con ID {id}.", id);
            try
            {
                var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
                if (libro == null)
                {
                    _logger.LogWarning("Situación inesperada: No se encontró el libro con ID {id} para mostrar la vista de confirmación de eliminación.", id);
                    return NotFound("Libro no encontrado");
                }
                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar el libro con ID {id} previo a la eliminación.", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Iniciando operación de eliminación (POST): Confirmando baja definitiva del libro con ID {id}.", id);
            try
            {
                var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
                if (libro == null)
                {
                    _logger.LogWarning("Situación inesperada: Intento de eliminar un libro que ya no existe en el sistema con ID {id}.", id);
                    return NotFound("Libro no encontrado");
                }

                _context.Libros.Remove(libro);
                _context.SaveChanges();

                _logger.LogInformation("Operación exitosa: El libro con ID {id} ha sido removido de la base de datos.", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al ejecutar la baja definitiva del libro con ID {id}.", id);
                throw;
            }
        }
    }
}
