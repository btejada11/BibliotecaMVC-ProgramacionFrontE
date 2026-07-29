using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private static List<Autor> _autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6), Activo = false },
            new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
            new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28), Activo = true }
        };
        public IActionResult Index()
        {
            return View(_autores);
        }

        public IActionResult Details(int id)
        {
            var autor = _autores.FirstOrDefault(x => x.Id == id);
            if (autor == null)
            {
                return NotFound("Autor no encontrado");
            }
            return View(autor);

        }
        public IActionResult Edit(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor == null)
            {
                return NotFound("Autor no encontrado");
            }
            return View(autor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            var autorExistente = _autores.FirstOrDefault(a => a.Id == autor.Id);
            if (autorExistente == null)
            {
                return NotFound("Autor no encontrado");
            }
            autorExistente.Id = autor.Id;
            autorExistente.Nombre = autor.Nombre;
            autorExistente.Apellido = autor.Apellido;
            autorExistente.Nacionalidad = autor.Nacionalidad;
            autorExistente.FechaNacimiento = autor.FechaNacimiento;
            autorExistente.Activo = autor.Activo;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            autor.Activo = true;

            if (_autores.Any())
            {
                autor.Id = _autores.Max(x => x.Id) + 1;
            }
            else
            {
                autor.Id = 1;
            }
            _autores.Add(autor);

            return RedirectToAction(nameof(Index));

        }
        public IActionResult Delete(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor == null)
            {
                return NotFound("Autor no encontrado");
            }
            return View(autor);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteDeVelda(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor != null)
            {
                _autores.Remove(autor);
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
    
