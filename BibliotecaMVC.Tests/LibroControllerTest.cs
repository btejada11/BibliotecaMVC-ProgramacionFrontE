// Controlador que se va a probar.
using BibliotecaMVC.Controllers;
// Modelo Libro.
using BibliotecaMVC.Models;
// Aquí está IRepositorioLibro (debe existir en el proyecto BibliotecaMVC, no en el de pruebas).
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

public class LibrosControllerTests
{
    // Repositorio simulado: reemplaza al acceso real a la base de datos.
    private readonly Mock<IRepositorioLibro> _repo = new();

    // Crea el controlador con el repositorio simulado y un logger simulado.
    private LibrosController CreateController() =>
        new LibrosController(_repo.Object, new Mock<ILogger<LibrosController>>().Object);

    // Crea un libro válido para usar como dato de prueba.
    private static Libro NuevoLibro(int id = 1, string titulo = "Rayuela") => new()
    {
        ID = id,
        Titulo = titulo,
        Autor = "Julio Cortázar",
        Categoria = "Novela",
        Precio = 12.50m,
        Disponible = true
    };

    // ---------- Index ----------

    [Fact]
    public void Index_ConLibros_DevuelveLaListaDeLibros()
    {
        _repo.Setup(s => s.ObtenerTodos()).Returns(new List<Libro>
        {
            NuevoLibro(1, "Libro A"),
            NuevoLibro(2, "Libro B")
        });

        var result = CreateController().Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Equal(2, libros.Count());
    }

    [Fact]
    public void Index_SinLibros_DevuelveListaVacia()
    {
        _repo.Setup(s => s.ObtenerTodos()).Returns(new List<Libro>());

        var result = CreateController().Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Empty(libros);
    }

    [Fact]
    public void Index_CuandoElServicioFalla_RelanzaLaExcepcion()
    {
        _repo.Setup(s => s.ObtenerTodos()).Throws(new InvalidOperationException("fallo"));

        Assert.Throws<InvalidOperationException>(() => CreateController().Index());
    }

    // ---------- Details ----------

    [Fact]
    public void Details_ConLibroExistente_DevuelveElLibro()
    {
        var libro = NuevoLibro(1);
        _repo.Setup(s => s.ObtenerLibroPorId(1)).Returns(libro);

        var result = CreateController().Details(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.NotNull(view.Model);
        Assert.Same(libro, view.Model);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public void Details_ConLibroInexistente_DevuelveNotFound(int id)
    {
        _repo.Setup(s => s.ObtenerLibroPorId(id)).Returns((Libro)null!);

        var result = CreateController().Details(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Libro no encontrado", notFound.Value);
    }

    // ---------- Create ----------

    [Fact]
    public void CreateGet_DevuelveLaVista()
    {
        var result = CreateController().Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void CreatePost_ConLibroValido_LoMarcaDisponibleLoGuardaYRedirige()
    {
        var libro = NuevoLibro(0, "El Aleph");
        libro.Disponible = false; // el controlador debe forzarlo a true

        var result = CreateController().Create(libro);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.True(libro.Disponible);
        _repo.Verify(s => s.Agregar(libro), Times.Once);
    }

    [Fact]
    public void CreatePost_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        var libro = NuevoLibro(0, "");

        var result = controller.Create(libro);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
        Assert.False(controller.ModelState.IsValid);
        _repo.Verify(s => s.Agregar(It.IsAny<Libro>()), Times.Never);
    }

    // ---------- Edit (GET) ----------

    [Fact]
    public void EditGet_ConLibroExistente_DevuelveLaVistaConElLibro()
    {
        var libro = NuevoLibro(1);
        _repo.Setup(s => s.ObtenerLibroPorId(1)).Returns(libro);

        var result = CreateController().Edit(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
    }

    [Fact]
    public void EditGet_ConLibroInexistente_DevuelveNotFound()
    {
        _repo.Setup(s => s.ObtenerLibroPorId(999)).Returns((Libro)null!);

        var result = CreateController().Edit(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ---------- Edit (POST) ----------

    [Fact]
    public void EditPost_ConLibroValido_ActualizaYRedirigeAlIndice()
    {
        var editado = NuevoLibro(1, "Título actualizado");
        _repo.Setup(s => s.ObtenerLibroPorId(1)).Returns(NuevoLibro(1, "Título original"));

        var result = CreateController().Edit(editado);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        _repo.Verify(s => s.Actualizar(editado), Times.Once);
    }

    [Fact]
    public void EditPost_ConModeloInvalido_NoActualizaYDevuelveLaVista()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        var libro = NuevoLibro(1, "");

        var result = controller.Edit(libro);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
        _repo.Verify(s => s.Actualizar(It.IsAny<Libro>()), Times.Never);
    }

    [Fact]
    public void EditPost_ConLibroInexistente_DevuelveNotFound()
    {
        _repo.Setup(s => s.ObtenerLibroPorId(999)).Returns((Libro)null!);

        var result = CreateController().Edit(NuevoLibro(999));

        Assert.IsType<NotFoundObjectResult>(result);
        _repo.Verify(s => s.Actualizar(It.IsAny<Libro>()), Times.Never);
    }

    // ---------- Delete (GET: confirmación) ----------

    [Fact]
    public void DeleteGet_ConLibroExistente_DevuelveLaVistaConElLibro()
    {
        var libro = NuevoLibro(1);
        _repo.Setup(s => s.ObtenerLibroPorId(1)).Returns(libro);

        var result = CreateController().Delete(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
    }

    [Fact]
    public void DeleteGet_ConLibroInexistente_DevuelveNotFound()
    {
        _repo.Setup(s => s.ObtenerLibroPorId(999)).Returns((Libro)null!);

        var result = CreateController().Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ---------- DeleteConfirmed (POST) ----------

    [Fact]
    public void DeleteConfirmed_ConLibroExistente_EliminaYRedirigeAlIndice()
    {
        _repo.Setup(s => s.ObtenerLibroPorId(1)).Returns(NuevoLibro(1));

        var result = CreateController().DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        _repo.Verify(s => s.Eliminar(1), Times.Once);
    }

    [Fact]
    public void DeleteConfirmed_ConLibroInexistente_DevuelveNotFound()
    {
        _repo.Setup(s => s.ObtenerLibroPorId(999)).Returns((Libro)null!);

        var result = CreateController().DeleteConfirmed(999);

        Assert.IsType<NotFoundObjectResult>(result);
        _repo.Verify(s => s.Eliminar(It.IsAny<int>()), Times.Never);
    }
}