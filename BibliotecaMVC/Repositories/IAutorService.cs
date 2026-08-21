using BibliotecaMVC.Models;
using System.Collections.Generic;

namespace BibliotecaMVC.Repositories
{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerTodos();
        Autor ObtenerAutorPorId(int id);
        void Agregar(Autor autor);
        void Actualizar(Autor autor);
        void Eliminar(int id);

    }
}
