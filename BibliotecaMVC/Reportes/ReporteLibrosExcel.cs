using BibliotecaMVC.Models;
using ClosedXML.Excel;

namespace BibliotecaMVC.Reportes
{
    public static class ReporteLibrosExcel
    {
        public static byte[] Generar(IReadOnlyList<Libro> libros, string? autor, string? categoria, DateTime fechaGeneracion)
        {
            var colorPrimario = XLColor.FromHtml("#1F4E79");
            var colorZebra = XLColor.FromHtml("#F2F2F2");

            var autorTexto = string.IsNullOrWhiteSpace(autor) ? "Todos" : autor;
            var categoriaTexto = string.IsNullOrWhiteSpace(categoria) ? "Todas" : categoria;
            var disponibles = libros.Count(l => l.Disponible);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Reporte de Libros");

            // Título, fecha, filtros y resumen
            ws.Cell("A1").Value = "BibliotecaMVC - Reporte de Libros";
            ws.Range("A1:F1").Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 16;
            ws.Cell("A1").Style.Font.FontColor = colorPrimario;

            ws.Cell("A2").Value = $"Fecha de generación: {fechaGeneracion:dd/MM/yyyy HH:mm}";
            ws.Cell("A3").Value = $"Filtros - Autor: {autorTexto} | Categoría: {categoriaTexto}";
            ws.Cell("A4").Value = $"Total de libros: {libros.Count}";
            ws.Cell("A5").Value = $"Disponibles: {disponibles} | No disponibles: {libros.Count - disponibles}";
            ws.Range("A4:A5").Style.Font.Bold = true;

            // Encabezados de la tabla
            const int filaEncabezado = 7;
            var encabezados = new[] { "ID", "Título", "Autor", "Categoría", "Precio", "Disponible" };
            for (var i = 0; i < encabezados.Length; i++)
                ws.Cell(filaEncabezado, i + 1).Value = encabezados[i];

            var rangoEncabezado = ws.Range(filaEncabezado, 1, filaEncabezado, encabezados.Length);
            rangoEncabezado.Style.Font.Bold = true;
            rangoEncabezado.Style.Font.FontColor = XLColor.White;
            rangoEncabezado.Style.Fill.BackgroundColor = colorPrimario;
            rangoEncabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rangoEncabezado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Datos (vienen de la base de datos)
            var fila = filaEncabezado + 1;
            for (var i = 0; i < libros.Count; i++)
            {
                var libro = libros[i];

                ws.Cell(fila, 1).Value = libro.ID;
                ws.Cell(fila, 2).Value = libro.Titulo ?? string.Empty;
                ws.Cell(fila, 3).Value = libro.Autor ?? string.Empty;
                ws.Cell(fila, 4).Value = libro.Categoria ?? string.Empty;
                ws.Cell(fila, 5).Value = libro.Precio;
                ws.Cell(fila, 5).Style.NumberFormat.Format = "$#,##0.00";
                ws.Cell(fila, 6).Value = libro.Disponible ? "Sí" : "No";

                ws.Cell(fila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell(fila, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                if (i % 2 == 1)
                    ws.Range(fila, 1, fila, encabezados.Length).Style.Fill.BackgroundColor = colorZebra;

                fila++;
            }

            var ultimaFila = fila - 1;

            // Bordes, filtro automático, panel inmovilizado y ancho de columnas
            var rangoTabla = ws.Range(filaEncabezado, 1, ultimaFila, encabezados.Length);
            rangoTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoTabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rangoTabla.SetAutoFilter();

            ws.SheetView.FreezeRows(filaEncabezado);
            ws.Columns().AdjustToContents(filaEncabezado, ultimaFila);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}