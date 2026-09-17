using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para operaciones de EF Core
using BibliotecaMVC.Models;
using BibliotecaMVC.Data; // Asegura el acceso a tu BibliotecaContext
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        // 1. Reemplazamos el repositorio por el DbContext nativo
        private readonly BibliotecaContext _context;

        // 2. El contexto se recibe por inyección de dependencias
        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }

        // Muestra la lista de libros (Actividad 2)
        public IActionResult Index()
        {
            // Consultamos la tabla directamente desde la base de datos SQL Server
            var libros = _context.Libros.ToList();
            return View(libros);
        }

        // Muestra el detalle de un libro
        public IActionResult Details(int id)
        {
            // Buscamos el libro directamente en el DbSet de la base de datos
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Muestra el formulario de creación (Actividad 3)
        public IActionResult Create()
        {
            return View();
        }

        // Procesa la creación del libro (Actividad 3)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            // Mantenemos tu lógica original para el estado por defecto
            libro.Disponible = true;

            // 3. Reemplazamos la llamada del repositorio por los métodos nativos de EF Core
            _context.Libros.Add(libro); // Prepara la entidad en memoria
            _context.SaveChanges();     // Envía y guarda los cambios reales en SQL Server

            return RedirectToAction(nameof(Index));
        }

        // Muestra el formulario de edición
        public IActionResult Edit(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Procesa la edición del libro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            // Buscamos la entidad existente en la base de datos para modificarla
            var libroExistente = _context.Libros.FirstOrDefault(l => l.ID == libro.ID);
            if (libroExistente == null)
            {
                return NotFound("Libro no encontrado");
            }

            // Mapeamos los campos de tu formulario al registro de la base de datos
            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Categoria = libro.Categoria;
            libroExistente.Precio = libro.Precio;
            libroExistente.Disponible = libro.Disponible;

            // Guardamos los cambios de la actualización
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // Muestra la vista de confirmación de eliminación
        public IActionResult Delete(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Procesa la eliminación definitiva
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro != null)
            {
                // Removemos el registro usando EF Core
                _context.Libros.Remove(libro);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
