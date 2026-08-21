using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using System.Collections.Generic;
using BibliotecaMVC.Repositories;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro _repositorio;

        // El repositorio se recibe por inyección de dependencias
        public LibrosController(IRepositorioLibro repositorio)
        {
            _repositorio = repositorio;
        }

        // Muestra la lista de libros
        public IActionResult Index()
        {
            var libros = _repositorio.ObtenerTodos();
            return View(libros);
        }

        // Muestra el detalle de un libro
        public IActionResult Details(int id)
        {
            var libro = _repositorio.ObtenerLibroPorId(id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        // Muestra el formulario de creación
        public IActionResult Create()
        {
            return View();
        }

        // Procesa la creación del libro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            libro.Disponible = true;

            // Usamos el método del repositorio para agregar (la asignación de ID debe ir en el repo o base de datos)
            _repositorio.Agregar(libro);

            return RedirectToAction(nameof(Index));
        }

        // Muestra el formulario de edición
        public IActionResult Edit(int id)
        {
            var libro = _repositorio.ObtenerLibroPorId(id);
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

            var libroExistente = _repositorio.ObtenerLibroPorId(libro.ID);
            if (libroExistente == null)
            {
                return NotFound("Libro no encontrado");
            }

            // Actualizamos los datos usando el repositorio
            _repositorio.Actualizar(libro);

            return RedirectToAction(nameof(Index));
        }

        // Muestra la vista de confirmación de eliminación
        public IActionResult Delete(int id)
        {
            var libro = _repositorio.ObtenerLibroPorId(id);
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
            var libro = _repositorio.ObtenerLibroPorId(id);
            if (libro != null)
            {
                _repositorio.Eliminar(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
