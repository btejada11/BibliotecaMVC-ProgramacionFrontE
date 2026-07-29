using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        public static List<Libro> _libros = new List<Libro>
        {
            new Libro { ID = 1, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", Precio = 19.99m, Disponible = true },
            new Libro { ID = 2, Titulo = "La Casa de los Espíritus", Autor = "Isabel Allende", Categoria = "Novela", Precio = 14.99m, Disponible = true },
            new Libro { ID = 3, Titulo = "El Amor en los Tiempos del Cólera", Autor = "Gabriel García Márquez", Categoria = "Novela", Precio = 17.99m, Disponible = false }
        };

        public IActionResult Index()
        {
            return View(_libros);
        }
        public IActionResult Details(int id)
        {
            var libro = _libros.FirstOrDefault(x => x.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            libro.Disponible = true;

            if (_libros.Any())
            {
                libro.ID = _libros.Max(x => x.ID) + 1;
            }
            else
            {
                libro.ID = 1;
            }
            _libros.Add(libro);

            return RedirectToAction(nameof(Index));

        }
        public IActionResult Edit(int id)
        {
            var libro = _libros.FirstOrDefault(x => x.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            var libroExistente = _libros.FirstOrDefault(l => l.ID == libro.ID);
            if (libroExistente == null)
            {
                return NotFound("Libro no encontrado");
            }
            libroExistente.ID = libro.ID;
            libroExistente.Titulo = libro.Titulo;
            libroExistente.Autor = libro.Autor;
            libroExistente.Categoria = libro.Categoria;
            libroExistente.Precio = libro.Precio;
            libroExistente.Disponible = libro.Disponible;

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.ID == id);
            if (libro == null)
            {
                return NotFound("Libro no encontrado");
            }
            return View(libro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDeVelda(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.ID == id);
            if (libro != null)
            {
                _libros.Remove(libro);
            }
            return RedirectToAction(nameof(Index));

        }

    }
}
