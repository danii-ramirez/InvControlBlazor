using InvControl.Shared.DTO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InvControl.Server.Helpers
{
    public class StockReporteDocument : IDocument
    {
        private readonly List<StockReporteDTO> _data;

        public StockReporteDocument(List<StockReporteDTO> data)
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
                    column.Item().Text("Reporte de Stock")
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
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderCell).Text("Código").SemiBold();
                        header.Cell().Element(HeaderCell).Text("Nombre").SemiBold();
                        header.Cell().Element(HeaderCell).Text("Cantidad").SemiBold();
                    });

                    foreach (var item in _data)
                    {
                        table.Cell().Element(Cell).Text(item.Codigo.ToString());
                        table.Cell().Element(Cell).Text(item.Nombre);
                        table.Cell().Element(Cell).Text(item.Cantidad.ToString());
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
