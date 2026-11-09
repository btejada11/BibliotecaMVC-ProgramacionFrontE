using System.Globalization;
using BibliotecaMVC.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Reportes
{
    public static class ReporteLibrosPdf
    {
        private static readonly Color ColorPrimario = Colors.Blue.Darken3;
        private static readonly Color ColorZebra = Colors.Grey.Lighten4;

        public static byte[] Generar(IReadOnlyList<Libro> libros, string? autor, string? categoria, DateTime fechaGeneracion)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var autorTexto = string.IsNullOrWhiteSpace(autor) ? "Todos" : autor;
            var categoriaTexto = string.IsNullOrWhiteSpace(categoria) ? "Todas" : categoria;
            var filtros = $"Autor: {autorTexto}  |  Categoría: {categoriaTexto}";
            var disponibles = libros.Count(l => l.Disponible);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Encabezado: sistema + título del reporte
                    page.Header().Column(col =>
                    {
                        col.Item().Text("BibliotecaMVC").FontSize(11).FontColor(Colors.Grey.Darken1);
                        col.Item().Text("Reporte de Libros").FontSize(22).Bold().FontColor(ColorPrimario);
                        col.Item().Text(filtros).FontSize(10).FontColor(Colors.Grey.Darken2);
                        col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(ColorPrimario);
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(12);

                        if (libros.Count == 0)
                        {
                            col.Item().Text("No se encontraron libros con los filtros aplicados.").Italic();
                        }
                        else
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(35);  // ID
                                    c.RelativeColumn(3);   // Título
                                    c.RelativeColumn(2);   // Autor
                                    c.RelativeColumn(2);   // Categoría
                                    c.ConstantColumn(55);  // Precio
                                    c.ConstantColumn(65);  // Disponible
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(Encabezado).Text("ID");
                                    header.Cell().Element(Encabezado).Text("Título");
                                    header.Cell().Element(Encabezado).Text("Autor");
                                    header.Cell().Element(Encabezado).Text("Categoría");
                                    header.Cell().Element(Encabezado).AlignRight().Text("Precio");
                                    header.Cell().Element(Encabezado).AlignCenter().Text("Disponible");
                                });

                                for (var i = 0; i < libros.Count; i++)
                                {
                                    var libro = libros[i];
                                    var indice = i;

                                    table.Cell().Element(c => Celda(c, indice)).Text(libro.ID.ToString());
                                    table.Cell().Element(c => Celda(c, indice)).Text(libro.Titulo);
                                    table.Cell().Element(c => Celda(c, indice)).Text(libro.Autor);
                                    table.Cell().Element(c => Celda(c, indice)).Text(libro.Categoria ?? "-");
                                    table.Cell().Element(c => Celda(c, indice)).AlignRight()
                                        .Text("$" + libro.Precio.ToString("N2", CultureInfo.InvariantCulture));
                                    table.Cell().Element(c => Celda(c, indice)).AlignCenter()
                                        .Text(libro.Disponible ? "Sí" : "No");
                                }
                            });
                        }

                        // Resumen
                        col.Item().Border(1).BorderColor(ColorPrimario)
                            .Background(Colors.Blue.Lighten5).Padding(8).Column(r =>
                            {
                                r.Item().Text("Resumen").Bold().FontColor(ColorPrimario);
                                r.Item().Text($"Total de libros: {libros.Count}");
                                r.Item().Text($"Disponibles: {disponibles}  |  No disponibles: {libros.Count - disponibles}");
                            });
                    });

                    // Pie de página: fecha de generación + numeración
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem()
                            .Text($"Generado el {fechaGeneracion:dd/MM/yyyy HH:mm}")
                            .FontSize(9).FontColor(Colors.Grey.Darken1);

                        row.RelativeItem().AlignRight().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1));
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                });
            }).GeneratePdf();
        }

        private static IContainer Encabezado(IContainer container) =>
            container.Background(ColorPrimario)
                     .Padding(5)
                     .DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White));

        private static IContainer Celda(IContainer container, int indice) =>
            container.Background(indice % 2 == 0 ? Colors.White : ColorZebra)
                     .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                     .Padding(5);
    }
}