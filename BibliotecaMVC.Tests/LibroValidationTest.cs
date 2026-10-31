// Importa las clases que permiten ejecutar las validaciones de Data Annotations.
using System.ComponentModel.DataAnnotations;
// Importa el modelo Libro de la aplicación principal.
using BibliotecaMVC.Models;

// Define el espacio de nombres del proyecto de pruebas.
namespace BibliotecaMVC.Tests;

// Agrupa las pruebas relacionadas con las reglas de validación de Libro.
public class LibroValidationTests
{
    // Comprueba que el título sea obligatorio con distintos valores inválidos.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TituloEsObligatorio(string? titulo)
    {
        // Crea un libro válido y solo invalida el título.
        var libro = CrearLibroValido();
        libro.Titulo = titulo!;

        // Ejecuta las validaciones definidas mediante atributos en Libro.
        var errores = Validate(libro);

        // Comprueba que exista un error asociado a Titulo con el mensaje esperado.
        var error = Assert.Single(errores);
        Assert.Contains(nameof(Libro.Titulo), error.MemberNames);
        Assert.Equal("El título es obligatorio.", error.ErrorMessage);
    }

    // Comprueba que el autor sea obligatorio con distintos valores inválidos.
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AutorEsObligatorio(string? autor)
    {
        // Crea un libro válido y solo invalida el autor.
        var libro = CrearLibroValido();
        libro.Autor = autor!;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que exista un error asociado a Autor con el mensaje esperado.
        var error = Assert.Single(errores);
        Assert.Contains(nameof(Libro.Autor), error.MemberNames);
        Assert.Equal("El autor es obligatorio.", error.ErrorMessage);
    }

    // Comprueba que se reporten ambos errores cuando faltan título y autor.
    [Fact]
    public void SinTituloNiAutor_GeneraDosErrores()
    {
        // Crea un libro con los dos campos obligatorios vacíos.
        var libro = CrearLibroValido();
        libro.Titulo = string.Empty;
        libro.Autor = string.Empty;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que haya exactamente dos errores.
        Assert.Equal(2, errores.Count);
    }

    // Comprueba que un libro con todos los datos correctos no genere errores.
    [Fact]
    public void LibroValido_NoGeneraErrores()
    {
        // Ejecuta las validaciones sobre un libro válido.
        var errores = Validate(CrearLibroValido());

        // Comprueba que no exista ningún error de validación.
        Assert.Empty(errores);
    }

    // Documenta el comportamiento actual: la categoría no es obligatoria.
    [Fact]
    public void CategoriaVacia_NoGeneraError()
    {
        // Crea un libro válido con la categoría vacía.
        var libro = CrearLibroValido();
        libro.Categoria = string.Empty;

        // Ejecuta las validaciones del modelo.
        var errores = Validate(libro);

        // Comprueba que no haya errores asociados a Categoria.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Libro.Categoria)));
    }

    // Crea un libro con todos los campos obligatorios válidos.
    private static Libro CrearLibroValido() => new()
    {
        Titulo = "Cien años de soledad",
        Autor = "Gabriel García Márquez",
        Categoria = "Novela",
        Precio = 15.50m,
        Disponible = true
    };

    // Método auxiliar que concentra la ejecución de las validaciones del modelo.
    private static IReadOnlyCollection<ValidationResult> Validate(Libro libro)
    {
        // Crea el contexto de validación para la instancia recibida.
        var context = new ValidationContext(libro);
        // Prepara la colección donde se almacenarán los errores encontrados.
        var errores = new List<ValidationResult>();
        // Ejecuta todas las validaciones, incluso las propiedades no obligatorias.
        Validator.TryValidateObject(libro, context, errores, validateAllProperties: true);
        // Devuelve los errores para que cada prueba pueda analizarlos.
        return errores;
    }
}