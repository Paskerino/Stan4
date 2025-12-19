using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Stand4
{
    /// <summary>
    /// Interaction logic for ReportDialogView.xaml
    /// </summary>
    public partial class ReportDialogView : Window
    {
        public ReportDialogView(ReportViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Підписуємося на подію завантаження, щоб побудувати таблицю
            Loaded += (s, e) => BuildTable(viewModel);
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            // Чекаємо ініціалізації ядра
            await PdfViewer.EnsureCoreWebView2Async();

            // Налаштовуємо панель інструментів PDF
            // Тут можна приховати зайві кнопки, якщо вони вам не треба
            PdfViewer.CoreWebView2.Settings.HiddenPdfToolbarItems =
                Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.Bookmarks |
                Microsoft.Web.WebView2.Core.CoreWebView2PdfToolbarItems.Search;

            // Кнопка FullScreen тут теж є, але програмно її натиснути не можна.
        }
        public async Task PrintCurrentPdf()
        {
            // Це відкриє стандартне системне вікно друку поверх WebView2
            await PdfViewer.CoreWebView2.ExecuteScriptAsync("window.print();");
        }
        private void BuildTable(ReportViewModel vm)
        {
            //DynamicRowsGroup.Rows.Clear();

            //if (vm.DataRows == null) return;

            //foreach (var item in vm.DataRows)
            //{
            //    var row = new TableRow();

            //    // Оскільки у вас GenericReportData, припускаємо, що Cells[0] - назва, Cells[1] - значення тощо.
            //    // Або адаптуйте це під вашу логіку.
            //    foreach (var cellData in item.Cells)
            //    {
            //        row.Cells.Add(CreateCell(cellData.Value));
            //    }

            //    DynamicRowsGroup.Rows.Add(row);
            //}
        }

        private TableCell CreateCell(string text)
        {
            return new TableCell(new Paragraph(new Run(text)))
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Padding = new Thickness(5)
            };
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Це викличе стандартний діалог друку.
                // Якщо користувач вибере "Microsoft Print to PDF" -> отримає PDF.
                // Якщо вибере принтер -> отримає папір.

                // Отримуємо документ з в'ювера
                //var doc = DocViewer.Document;

                //// Важливо: налаштування ширини сторінки під принтер
                //var paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;

                //printDialog.PrintDocument(paginator, "Test Report Stand4");
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
