using BibliotecaMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaMVC.Repositories
{
    public class ReposiorioEnMemoria : IRepositorioLibro
    {
        // Creamos una lista estática para que los datos persistan entre peticiones
        private static readonly List<Libro> _libros = new List<Libro>
        {
            new Libro { ID = 1, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", Precio = 19.99m, Disponible = true },
            new Libro { ID = 2, Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Categoria = "Novela", Precio = 14.99m, Disponible = true },
            new Libro { ID = 3, Titulo = "El Principito", Autor = "Antoine de Saint-Exupéry", Categoria = "Fábula", Precio = 9.99m, Disponible = false },
            new Libro { ID = 4, Titulo = "1984", Autor = "George Orwell", Categoria = "Distopía", Precio = 12.99m, Disponible = true },
            new Libro { ID = 5, Titulo = "Moby Dick", Autor = "Herman Melville", Categoria = "Aventura", Precio = 15.99m, Disponible = false }
        };

        // 1. Obtener todos los libros
        public IEnumerable<Libro> ObtenerTodos()
        {
            return _libros;
        }

        // 2. Buscar un libro por su ID
        public Libro ObtenerLibroPorId(int id)
        {
            return _libros.FirstOrDefault(l => l.ID == id);
        }

        // 3. Agregar un nuevo libro
        public void Agregar(Libro libro)
        {
            // Generamos un nuevo ID automáticamente sumando 1 al ID más alto
            libro.ID = _libros.Any() ? _libros.Max(l => l.ID) + 1 : 1;
            _libros.Add(libro);
        }

        // 4. Actualizar un libro existente
        public void Actualizar(Libro libro)
        {
            var libroExistente = ObtenerLibroPorId(libro.ID);
            if (libroExistente != null)
            {
                libroExistente.Titulo = libro.Titulo;
                libroExistente.Autor = libro.Autor;
                libroExistente.Categoria = libro.Categoria;
                libroExistente.Precio = libro.Precio;
                libroExistente.Disponible = libro.Disponible;
            }
        }

        // 5. Eliminar un libro
        public void Eliminar(int id)
        {
            var libro = ObtenerLibroPorId(id);
            if (libro != null)
            {
                _libros.Remove(libro);
            }
        }
    }
}
