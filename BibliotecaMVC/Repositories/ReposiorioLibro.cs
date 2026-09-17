using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioLibro : IRepositorioLibro
    {
        private readonly BibliotecaContext _context;

        // Inyectamos el DbContext de Entity Framework Core
        public RepositorioLibro(BibliotecaContext context)
        {
            _context = context;
        }

        // ACTIVIDAD 2: Mostrar Libros (Obtener de la Base de Datos)
        public IEnumerable<Libro> ObtenerTodos()
        {
            return _context.Libros.ToList();
        }

        public Libro ObtenerLibroPorId(int id)
        {
            return _context.Libros.FirstOrDefault(l => l.ID == id);
        }

        // ACTIVIDAD 3: Agregar Libro (Uso de Add y SaveChanges)
        public void Agregar(Libro libro)
        {
            _context.Libros.Add(libro); // Guarda el objeto en el contexto
            _context.SaveChanges();    // Impacta los cambios reales en SQL Server
        }

        // (Opcionales para más adelante, pero requeridos por la interfaz)
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
                _context.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var libro = ObtenerLibroPorId(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                _context.SaveChanges();
            }
        }
    }
}
