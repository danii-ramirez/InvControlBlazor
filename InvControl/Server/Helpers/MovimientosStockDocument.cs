using InvControl.Shared.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvControl.Server.Helpers
{
    public class MovimientosStockDocument : IDocument
    {
        private readonly List<MovimientoStockReporteDTO> _data;

        public MovimientosStockDocument(List<MovimientoStockReporteDTO> data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(column =>
                {
                    column.Item().Text("Reporte de Movimientos de Stock")
                        .FontSize(16)
                        .SemiBold()
                        .AlignCenter();

                    column.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(8)
                        .AlignRight();
                });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Movimiento");
                        header.Cell().Element(HeaderCell).Text("Código");
                        header.Cell().Element(HeaderCell).Text("Nombre");
                        header.Cell().Element(HeaderCell).Text("Cantidad");
                        header.Cell().Element(HeaderCell).Text("Fecha");
                        header.Cell().Element(HeaderCell).Text("Referencia");
                    });

                    foreach (var item in _data)
                    {
                        table.Cell().Element(Cell).Text(item.Movimiento);
                        table.Cell().Element(Cell).Text(item.Codigo.ToString());
                        table.Cell().Element(Cell).Text(item.Nombre);
                        table.Cell().Element(Cell).Text(item.Cantidad.ToString());
                        table.Cell().Element(Cell).Text(item.Fecha.ToString("dd/MM/yyyy"));
                        table.Cell().Element(Cell).Text(item.Referencia);
                    }
                });

                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        }

        private static IContainer HeaderCell(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Background(Colors.Grey.Lighten3)
                .Padding(5)
                .AlignMiddle();
        }

        private static IContainer Cell(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(5)
                .AlignMiddle();
        }
    }
}
