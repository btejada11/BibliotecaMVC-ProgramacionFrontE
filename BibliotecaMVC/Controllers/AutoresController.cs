using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories; // 1. Asegúrate de incluir el espacio de nombres de tu interfaz

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        // 2. Declaramos el servicio como una variable privada de solo lectura
        private readonly IAutorService _autorService;

        // 3. Modificamos el constructor para recibir la interfaz (Inyección de Dependencias)
        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IActionResult Index()
        {
            // Cambiado: Ahora le pide los datos al servicio
            var autores = _autorService.ObtenerTodos();
            return View(autores);
        }

        public IActionResult Details(int id)
        {
            // Cambiado: Busca usando el servicio
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound("Autor no encontrado");
            }
            return View(autor);
        }

        public IActionResult Edit(int id)
        {
            // Cambiado: Busca usando el servicio
            var autor = _autorService.ObtenerAutorPorId(id);
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

            // Cambiado: Primero verificamos si existe mediante el servicio
            var autorExistente = _autorService.ObtenerAutorPorId(autor.Id);
            if (autorExistente == null)
            {
                return NotFound("Autor no encontrado");
            }

            // Cambiado: Delegamos la actualización de los datos al servicio
            _autorService.Actualizar(autor);

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

            // Mantenemos la regla de negocio que tenías originalmente
            autor.Activo = true;

            // Cambiado: El servicio ahora se encarga de calcular el Id y guardarlo
            _autorService.Agregar(autor);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            // Cambiado: Busca usando el servicio
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound("Autor no encontrado");
            }
            return View(autor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Es buena práctica mantener o agregar esto por seguridad
        public IActionResult DeleteDeVelda(int id)
        {
            // Cambiado: Delegamos la eliminación directa al servicio
            _autorService.Eliminar(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
