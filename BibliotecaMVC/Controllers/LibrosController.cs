using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;

        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }

        // Actividad 2 y 3: Consulta permitida para todos (Admin, Bibliotecario, Usuario)
        public IActionResult Index()
        {
            var libros = _context.Libros.ToList();
            return View(libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Actividad 3.1: Muestra formulario de creación protegido
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // Actividad 3.2: CORREGIDO - Se añade Authorize para proteger el procesamiento del formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            libro.Disponible = true;
            _context.Libros.Add(libro);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Actividad 3.1: Muestra formulario de edición protegido
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Edit(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Actividad 3.2: CORREGIDO - Se añade Authorize para proteger el procesamiento de los cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var libroExistente = _context.Libros.FirstOrDefault(l => l.ID == libro.ID);
            if (libroExistente == null)
            {
                return NotFound("Libro no encontrado");
            }

            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Categoria = libro.Categoria;
            libroExistente.Precio = libro.Precio;
            libroExistente.Disponible = libro.Disponible;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Actividad 3.3: Muestra vista de confirmación de eliminación protegida
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult Delete(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Actividad 3.3: CORREGIDO - Se añade Authorize para restringir el borrado definitivo en el servidor
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotecario, Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = _context.Libros.FirstOrDefault(l => l.ID == id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
