using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

public class AutoresControllerTests
{
    private readonly Mock<IAutorService> _service = new();

    private AutoresController CreateController() =>
        new AutoresController(_service.Object, new Mock<ILogger<AutoresController>>().Object);

    // ---------- Index ----------

    [Fact]
    public void Index_DevuelveLaListaDeAutores()
    {
        _service.Setup(s => s.ObtenerTodos()).Returns(new List<Autor>
        {
            new() { Id = 1, Nombre = "Gabriel García Márquez", Nacionalidad = "Colombiana" },
            new() { Id = 2, Nombre = "Isabel Allende", Nacionalidad = "Chilena" }
        });

        var result = CreateController().Index();

        var view = Assert.IsType<ViewResult>(result);
        var autores = Assert.IsAssignableFrom<IEnumerable<Autor>>(view.Model);
        Assert.Equal(2, autores.Count());
    }

    [Fact]
    public void Index_CuandoElServicioFalla_RelanzaLaExcepcion()
    {
        _service.Setup(s => s.ObtenerTodos()).Throws(new InvalidOperationException("fallo"));

        Assert.Throws<InvalidOperationException>(() => CreateController().Index());
    }

    // ---------- Details ----------

    [Fact]
    public void Details_ConAutorExistente_DevuelveElAutor()
    {
        var autor = new Autor { Id = 1, Nombre = "Julio Cortázar", Nacionalidad = "Argentina" };
        _service.Setup(s => s.ObtenerAutorPorId(1)).Returns(autor);

        var result = CreateController().Details(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(autor, view.Model);
    }

    [Fact]
    public void Details_ConAutorInexistente_DevuelveNotFound()
    {
        _service.Setup(s => s.ObtenerAutorPorId(999)).Returns((Autor)null!);

        var result = CreateController().Details(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ---------- Edit (GET) ----------

    [Fact]
    public void EditGet_ConAutorExistente_DevuelveLaVistaConElAutor()
    {
        var autor = new Autor { Id = 1, Nombre = "Autor" };
        _service.Setup(s => s.ObtenerAutorPorId(1)).Returns(autor);

        var result = CreateController().Edit(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(autor, view.Model);
    }

    [Fact]
    public void EditGet_ConAutorInexistente_DevuelveNotFound()
    {
        _service.Setup(s => s.ObtenerAutorPorId(999)).Returns((Autor)null!);

        var result = CreateController().Edit(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ---------- Edit (POST) ----------

    [Fact]
    public void EditPost_ConAutorValido_ActualizaYRedirigeAlIndice()
    {
        var autor = new Autor { Id = 1, Nombre = "Nombre actualizado", Nacionalidad = "Española" };
        _service.Setup(s => s.ObtenerAutorPorId(1)).Returns(new Autor { Id = 1, Nombre = "Nombre original" });

        var result = CreateController().Edit(autor);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AutoresController.Index), redirect.ActionName);
        _service.Verify(s => s.Actualizar(autor), Times.Once);
    }

    [Fact]
    public void EditPost_ConModeloInvalido_NoActualizaYDevuelveLaVista()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError(nameof(Autor.Nombre), "El nombre es obligatorio.");
        var autor = new Autor { Id = 1 };

        var result = controller.Edit(autor);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(autor, view.Model);
        _service.Verify(s => s.Actualizar(It.IsAny<Autor>()), Times.Never);
    }

    [Fact]
    public void EditPost_ConAutorInexistente_DevuelveNotFound()
    {
        var autor = new Autor { Id = 999, Nombre = "Autor" };
        _service.Setup(s => s.ObtenerAutorPorId(999)).Returns((Autor)null!);

        var result = CreateController().Edit(autor);

        Assert.IsType<NotFoundObjectResult>(result);
        _service.Verify(s => s.Actualizar(It.IsAny<Autor>()), Times.Never);
    }

    // ---------- Create ----------

    [Fact]
    public void CreateGet_DevuelveLaVista()
    {
        var result = CreateController().Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void CreatePost_ConAutorValido_LoMarcaActivoLoGuardaYRedirige()
    {
        var autor = new Autor { Nombre = "Mario Vargas Llosa", Nacionalidad = "Peruana" };

        var result = CreateController().Create(autor);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AutoresController.Index), redirect.ActionName);
        Assert.True(autor.Activo);
        _service.Verify(s => s.Agregar(autor), Times.Once);
    }

    [Fact]
    public void CreatePost_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        var controller = CreateController();
        controller.ModelState.AddModelError(nameof(Autor.Nombre), "El nombre es obligatorio.");
        var autor = new Autor { Nacionalidad = "Chilena" };

        var result = controller.Create(autor);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(autor, view.Model);
        _service.Verify(s => s.Agregar(It.IsAny<Autor>()), Times.Never);
    }

    // ---------- Delete (GET: confirmación) ----------

    [Fact]
    public void DeleteGet_ConAutorExistente_DevuelveLaVistaConElAutor()
    {
        var autor = new Autor { Id = 1, Nombre = "Autor" };
        _service.Setup(s => s.ObtenerAutorPorId(1)).Returns(autor);

        var result = CreateController().Delete(1);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(autor, view.Model);
    }

    [Fact]
    public void DeleteGet_ConAutorInexistente_DevuelveNotFound()
    {
        _service.Setup(s => s.ObtenerAutorPorId(999)).Returns((Autor)null!);

        var result = CreateController().Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ---------- DeleteDeVelda (POST) ----------

    [Fact]
    public void DeleteDeVelda_ConAutorExistente_EliminaYRedirigeAlIndice()
    {
        _service.Setup(s => s.ObtenerAutorPorId(1)).Returns(new Autor { Id = 1 });

        var result = CreateController().DeleteDeVelda(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(AutoresController.Index), redirect.ActionName);
        _service.Verify(s => s.Eliminar(1), Times.Once);
    }

    [Fact]
    public void DeleteDeVelda_ConAutorInexistente_DevuelveNotFound()
    {
        _service.Setup(s => s.ObtenerAutorPorId(999)).Returns((Autor)null!);

        var result = CreateController().DeleteDeVelda(999);

        Assert.IsType<NotFoundObjectResult>(result);
        _service.Verify(s => s.Eliminar(It.IsAny<int>()), Times.Never);
    }
}