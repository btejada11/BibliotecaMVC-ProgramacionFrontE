using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace BibliotecaMVC.Models

{
    public class Libro
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; }
        [Required(ErrorMessage = "El autor es obligatorio.")]
        public string Autor { get; set; }
        public string Categoria { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }
        public bool Disponible { get; set; }

    }
}
