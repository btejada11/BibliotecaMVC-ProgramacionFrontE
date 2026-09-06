using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using System;
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

        // GET: Categorias
        public IActionResult Index()
        {
            var categorias = new List<Categoria>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT ID, Nombre, Descripcion FROM Categorias";
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

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
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
            TempData["SuccessMessage"] = "Categoría creada exitosamente.";
            return RedirectToAction("Index");
        }

        // GET: Categorias/Edit/5
        public IActionResult Edit(int id)
        {
            Categoria categoria = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            categoria = new Categoria
                            {
                                ID = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            };
                        }
                    }
                }
            }

            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre de la categoría es obligatorio.");
                return View(categoria);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @ID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", categoria.ID);
                    command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Categoría actualizada exitosamente.";
            return RedirectToAction("Index");
        }

        // POST: Categorias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Categorias WHERE ID = @ID";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Categoría eliminada exitosamente.";
            return RedirectToAction("Index");
        }
    }
}
