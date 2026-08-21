using BibliotecaMVC.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BibliotecaMVC.Repositories
{
    public class AutorEnMemoria : IAutorService
    {
        private readonly List<Autor> _autores;
        public AutorEnMemoria()
        {
            _autores = new List<Autor>
            {
                new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
                new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
                new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28), Activo = true }
            };
        }
        public IEnumerable<Autor> ObtenerTodos()
        {
            return _autores;
        }
        public Autor ObtenerAutorPorId(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
        }
        public void Agregar(Autor autor)
        {
            autor.Id = _autores.Max(a => a.Id) + 1;
            _autores.Add(autor);
        }
        public void Actualizar(Autor autor)
        {
            var autorExistente = ObtenerAutorPorId(autor.Id);
            if (autorExistente != null)
            {
                autorExistente.Nombre = autor.Nombre;
                autorExistente.Apellido = autor.Apellido;
                autorExistente.Nacionalidad = autor.Nacionalidad;
                autorExistente.FechaNacimiento = autor.FechaNacimiento;
                autorExistente.Activo = autor.Activo;
            }
        }
        public void Eliminar(int id)
        {
            var autorAEliminar = ObtenerAutorPorId(id);
            if (autorAEliminar != null)
            {
                _autores.Remove(autorAEliminar);
            }
        }
    }
}
