using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        // Se inyecta ILogger conservando la estructura actual y respetando la dependencia de IConfiguration
        public CategoriasController(IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
            _logger = logger;
        }

        // GET: Categorias
        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando operación de consulta: Solicitando el listado completo de categorías.");
            var categorias = new List<Categoria>();

            try
            {
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
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error de SQL Server al intentar obtener el listado de categorías.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico general en el módulo de Categorías al listar los registros.");
                throw;
            }
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Accediendo al formulario de creación de categorías (GET).");
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation("Iniciando operación de creación (POST): Registrando una nueva categoría.");

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("El modelo enviado para la creación de categoría no es válido debido a datos obligatorios faltantes.");
                ModelState.AddModelError("Nombre", "El nombre de la categoría es obligatorio.");
                return View(categoria);
            }

            try
            {
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

                _logger.LogInformation("Operación exitosa: Categoría '{Nombre}' creada correctamente en la base de datos.", categoria.Nombre);
                TempData["SuccessMessage"] = "Categoría creada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar insertar la nueva categoría en el almacenamiento de datos.");
                throw;
            }
        }

        // GET: Categorias/Edit/5
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando flujo de edición (GET): Buscando categoría con ID {id} para cargar formulario.", id);
            Categoria categoria = null;

            try
            {
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
                    _logger.LogWarning("Situación inesperada: No se encontró la categoría con ID {id} para editar.", id);
                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar recuperar la categoría con ID {id} para su edición.", id);
                throw;
            }
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            _logger.LogInformation("Iniciando operación de edición (POST): Procesando actualización de la categoría con ID {id}.", categoria.ID);

            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("El modelo enviado para modificar la categoría con ID {id} tiene un nombre inválido o vacío.", categoria.ID);
                ModelState.AddModelError("Nombre", "El nombre de la categoría es obligatorio.");
                return View(categoria);
            }

            try
            {
                // Primero validamos si el registro existe en la BD antes de mandar el UPDATE
                bool existe = false;
                using (var connectionCheck = new SqlConnection(_connectionString))
                {
                    var sqlCheck = "SELECT COUNT(1) FROM Categorias WHERE ID = @ID";
                    using (var commandCheck = new SqlCommand(sqlCheck, connectionCheck))
                    {
                        commandCheck.Parameters.AddWithValue("@ID", categoria.ID);
                        connectionCheck.Open();
                        existe = Convert.ToInt32(commandCheck.ExecuteScalar()) > 0;
                    }
                }

                if (!existe)
                {
                    _logger.LogWarning("Situación inesperada: Intento fallido de editar una categoría inexistente con ID {id}.", categoria.ID);
                    return NotFound();
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

                _logger.LogInformation("Operación exitosa: Categoría con ID {id} modificada con éxito.", categoria.ID);
                TempData["SuccessMessage"] = "Categoría actualizada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al guardar los cambios de la categoría con ID {id}.", categoria.ID);
                throw;
            }
        }

        // POST: Categorias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando operación de eliminación (POST): Confirmando baja de la categoría con ID {id}.", id);

            try
            {
                // Comprobamos la existencia física del registro previo a la remoción
                bool existe = false;
                using (var connectionCheck = new SqlConnection(_connectionString))
                {
                    var sqlCheck = "SELECT COUNT(1) FROM Categorias WHERE ID = @ID";
                    using (var commandCheck = new SqlCommand(sqlCheck, connectionCheck))
                    {
                        commandCheck.Parameters.AddWithValue("@ID", id);
                        connectionCheck.Open();
                        existe = Convert.ToInt32(commandCheck.ExecuteScalar()) > 0;
                    }
                }
                if (!existe)
                {
                    _logger.LogWarning("Situación inesperada: Intento de eliminar una categoría que no existe en el sistema con ID {id}.", id);
                    return NotFound();
                }
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
                _logger.LogInformation("Operación exitosa: Categoría con ID {id} removida definitivamente de la base de datos.", id);
                TempData["SuccessMessage"] = "Categoría eliminada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error de integridad o ejecución al intentar borrar la categoría con ID {id}.", id);
                throw;
            }
        }
    }
}
