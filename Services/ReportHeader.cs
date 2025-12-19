using CommonLogic.Core.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Stand4.Services
{
    // Реалізуємо IComponent для чистоти коду
    public class ReportHeader : IComponent
    {
        private readonly GenericReportData _data;

        public ReportHeader(GenericReportData data)
        {
            _data = data;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                // 1. Головний заголовок по центру
                column.Item().PaddingBottom(10).AlignCenter().Text(_data.ReportName)
                      .FontSize(20).Bold().FontColor(Colors.Black);

                // 2. Таблиця з інформацією (з рамками)
                column.Item().Table(table =>
                {
                    // Визначаємо 4 колонки:
                    // Label (вузька) | Value (широка) | Label (вузька) | Value (широка)
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80);  // Ширина назви поля
                        columns.RelativeColumn();    // Ширина значення
                        columns.ConstantColumn(80);
                        columns.RelativeColumn();
                    });

                    // --- Рядок 1: Замовник та Дата ---
                    // Використовуємо метод CellStyle для спільних стилів
                    table.Cell().Element(LabelStyle).Text("Замовник:");
                    table.Cell().Element(ValueStyle).Text(_data.Metadata["OperatorCode"]);

                    table.Cell().Element(LabelStyle).Text(_data.Metadata["repDate"]);
                    table.Cell().Element(ValueStyle).Text(_data.Metadata["StartTime"]);

                    // --- Рядок 2: Виріб та Серійний номер ---
                    table.Cell().Element(LabelStyle).Text("Виріб:");
                    table.Cell().Element(ValueStyle).Text(_data.Metadata["OperatorCode"]);

                    table.Cell().Element(LabelStyle).Text("Зав. №:");
                    table.Cell().Element(ValueStyle).Text(_data.Metadata["SerialNumber"]);

                    // --- Рядок 3: Оператор (об'єднуємо останні 2 комірки, якщо треба) ---
                    table.Cell().Element(LabelStyle).Text("Оператор:");
                    table.Cell().ColumnSpan(3).Element(ValueStyle).Text(_data.Metadata["OperatorCode"]);

                    // --- Рядок 4: РЕЗУЛЬТАТ (Великий блок) ---
                   // table.Cell().ColumnSpan(4).PaddingTop(10).Element(ResultBlock);
                });
            });
        }

        // --- Стилі (C# 7.3 - звичайні методи) ---

        // Стиль для назви поля (сірий фон, жирний шрифт, рамка)
        private IContainer LabelStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .Background(Colors.Grey.Lighten3)
                .Padding(5)
                .AlignLeft()
                .AlignMiddle();
        }

        // Стиль для значення (білий фон, рамка)
        private IContainer ValueStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .Padding(5)
                .AlignLeft()
                .AlignMiddle();
        }

        // Логіка відображення результату (Зелений/Червоний)
        private void ResultBlock(IContainer container)
        {
            string resultText = _data.IsSuccess ? "ПРИДАТНИЙ" : "БРАК";
            string color = _data.IsSuccess ? Colors.Green.Medium : Colors.Red.Medium;

            container
                .Border(2) // Товстіша рамка
                .BorderColor(color)
                .Background(_data.IsSuccess ? Colors.Green.Lighten5 : Colors.Red.Lighten5)
                .Padding(10)
                .AlignCenter()
                .Text(text =>
                {
                    text.Span("РЕЗУЛЬТАТ: ").FontSize(14).Bold();
                    text.Span(resultText).FontSize(20).Bold().FontColor(color);
                });
        }
    }
}