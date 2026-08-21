using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public interface IRepositorioLibro
    {
        IEnumerable<Libro> ObtenerTodos();
        Libro ObtenerLibroPorId(int id);
        void Agregar(Libro libro);
        void Actualizar(Libro libro);
        void Eliminar(int id);
    }
}
