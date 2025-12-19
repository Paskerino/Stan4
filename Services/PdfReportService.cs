using CommonLogic.Core.Models.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Stand4.Services;
using System.IO;
namespace Stand4
{
    public class PdfReportService
    {
        public void GeneratePdf(GenericReportData data, string filePath)
        {
            // Налаштування ліцензії (Community)
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // 1. Шапка
                   // page.Header().Element(header => ComposeHeader(header, data));
                    page.Header().Component(new ReportHeader(data));
                    // 2. Зміст (Таблиця)
                    page.Content().PaddingVertical(10).Element(content => ComposeTable(content, data));

                    // 3. Підвал
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" from ");
                        text.TotalPages();
                    });
                });
            })
            .GeneratePdf(filePath);
        }

        private void ComposeHeader(IContainer container, GenericReportData data)
        {
            container.Row(row =>
            {
                if (File.Exists("Assets/Stand4_clean_2.png"))
                {
                    row.ConstantItem(80).Image("Assets/Stand4_clean_2.png");
                }

                row.RelativeItem().PaddingLeft(10).Column(col =>
                {
                    col.Item().Text(data.Title).FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text(string.Format("Оператор: {0}", data.Title));
                    col.Item().Text(string.Format("Дата: {0}", data.Title));
                });
            });
        }

        private void ComposeTable(IContainer container, GenericReportData data)
        {
            container.Table(table =>
            {
                // Визначаємо ширину колонок
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Назва
                    columns.RelativeColumn(1); // Значення
                    columns.RelativeColumn(1); // Одиниця
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Заголовок таблиці
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Час");
                    header.Cell().Element(CellStyle).Text("Р1");
                    header.Cell().Element(CellStyle).Text("Р2");
                    header.Cell().Element(CellStyle).Text("Р3");
                    header.Cell().Element(CellStyle).Text("Р4");
                    header.Cell().Element(CellStyle).Text("Р5");
                });

                // Дані таблиці
                if (data.DataRows != null)
                {
                    foreach (var row in data.DataRows)
                    {
                        var date = row.Cells.Count > 0 ? row.Cells[0].Value : "-";
                        var p1 = row.Cells.Count > 1 ? row.Cells[1].Value : "-";
                        var p2 = row.Cells.Count > 2 ? row.Cells[2].Value : "-";
                        var p3 = row.Cells.Count > 2 ? row.Cells[2].Value : "-";
                        var p4 = row.Cells.Count > 2 ? row.Cells[2].Value : "-";
                        var p5 = row.Cells.Count > 2 ? row.Cells[2].Value : "-";

                        table.Cell().Element(DataStyle).Text(date);
                        table.Cell().Element(DataStyle).Text(p1);
                        table.Cell().Element(DataStyle).Text(p2);
                        table.Cell().Element(DataStyle).Text(p3);
                        table.Cell().Element(DataStyle).Text(p4);
                        table.Cell().Element(DataStyle).Text(p5);

                    }
                }
            });
        }


        private IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Black)
                .Padding(2)
                .DefaultTextStyle(x => x.SemiBold()); // Жирний шрифт для заголовків
        }

        private IContainer DataStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3)
                .Padding(2);
        }
    }
}