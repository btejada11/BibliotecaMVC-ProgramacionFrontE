using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BibliotecaMVC.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;

        public CategoriasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
        }

        public IActionResult Index()

        {
            var categorias = new List<Categoria>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Categorias";
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categorias.Add(new Categoria
                            {
                                ID = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return View(categorias);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre de la categoría es obligatorio.");
                return View(categoria);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            TempData["SuccessMessage"] = "Categoria creada exitosamente.";
            return RedirectToAction("Index");
        }
    }
}