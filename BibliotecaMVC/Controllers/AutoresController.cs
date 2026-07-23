using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            List<Autor> autores = new List<Autor>()
                {
                    new Autor
                {
                    Id = 1,
                    Nombre = "Robert",
                    Apellido = "Martin",
                    Nacionalidad = "Estadounidense",
                    fechaNacimiento = "1952-09-13",
                    Activo = true
                },
                new Autor
                {
                    Id = 2,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    Nacionalidad = "Colombiano",
                    fechaNacimiento = "1928-03-06",
                    Activo = true
                },
                new Autor
                {
                    Id = 3,
                    Nombre = "J.K.",
                    Apellido = "Rowling",
                    Nacionalidad = "Británica",
                    fechaNacimiento = "1965-07-31",
                    Activo = true
                },
                new Autor
                {
                    Id = 4,
                    Nombre = "George",
                    Apellido = "Orwell",
                    Nacionalidad = "Británico",
                    fechaNacimiento = "1903-06-25",
                    Activo = false
                },
                new Autor
                {
                    Id = 5,
                    Nombre = "Jane",
                    Apellido = "Austen",
                    Nacionalidad = "Británica",
                    fechaNacimiento = "1775-12-16",
                    Activo = false
                }
            };
            return View(autores);
        }
    }
}
    
