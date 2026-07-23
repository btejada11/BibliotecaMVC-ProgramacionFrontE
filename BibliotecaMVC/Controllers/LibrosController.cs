using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        public IActionResult Index()
        {
            List<Libro> libros = new List<Libro>()
            {
                new Libro
                {
                    ID = 1,
                    Titulo = "Clean Code",
                    Autor = "Robert C. Martin",
                    Categoria = "Programación",
                    Precio = 29.99m,
                    Disponible = true
                },
                new Libro
                {
                    ID = 2,
                    Titulo = "Cien años de soledad",
                    Autor = "Gabriel García Márquez",
                    Categoria = "Novela",
                    Precio = 19.99m,
                    Disponible = false
                },
            };
            return View(libros);
        }
    }
}
