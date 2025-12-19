using CommonLogic.Core.Models.Reports;
using PdfiumViewer;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Stand4
{
    public class ReportViewModel : BaseViewModel
    {
        private string _pdfFilePath;
        private readonly GenericReportData _reportData;
        private readonly PdfReportService _pdfService; // Сервіс з QuestPDF

        public ReportViewModel(GenericReportData reportData)
        {
            _reportData = reportData;
            _pdfService = new PdfReportService(); // Або через DI

            PrintPhysicalCommand = new RelayCommand(OnPrintPhysical);
            CloseCommand = new RelayCommand(OnClose);

            GenerateReport();
        }
        public ReportViewModel(GenericReportData reportData,string FilePath)
        {
            _reportData = reportData;

            PrintPhysicalCommand = new RelayCommand(OnPrintPhysical);
            CloseCommand = new RelayCommand(OnClose);

            PdfFilePath = FilePath;
            InitializeWebView();
        }
        

private async void InitializeWebView()
        {
         

            // Кнопка FullScreen тут теж є, але програмно її натиснути не можна.
        }

        // WebView2 підхопить цю зміну і покаже файл
        public string PdfFilePath
        {
            get => _pdfFilePath;
            set
            {
                _pdfFilePath = value;
                OnPropertyChanged();
            }
        }

        public ICommand PrintPhysicalCommand { get; }
        public ICommand CloseCommand { get; }

        private void GenerateReport()
        {
            // Генеруємо файл у тимчасову папку
           // string tempFile = Path.Combine(Path.GetTempPath(), $"report_{DateTime.Now.Ticks}.pdf");
           string tempDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Archive\\Reports\\{DateTime.Now:ddMMyyyy}\\");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            
            string tempFile = System.IO.Path.Combine(tempDir, $"Log_time{DateTime.Now:HH_mm_ss}.pdf");
            //$"Log_ID{_reportData.Metadata["OperatorCode"]}_Sn{_reportData.Metadata["SerialNumber"]}_time{DateTime.Now:HH_mm_ss}.pdf");
            // QuestPDF створює файл (код нижче)
            _pdfService.GeneratePdf(_reportData, tempFile);

            // Передаємо повний шлях у View
            PdfFilePath = tempFile;
        }

        private void OnPrintPhysical(object obj)
        {
            if (!File.Exists(PdfFilePath)) return;
            using (var document = PdfDocument.Load(PdfFilePath))
            using (var printDocument = document.CreatePrintDocument())
            {
                // Це налаштування бере принтер за замовчуванням автоматично
                printDocument.PrinterSettings.PrintToFile = false;

                // Можна задати ім'я принтера явно, якщо треба:
                // printDocument.PrinterSettings.PrinterName = "Samsung ML-1640";

                printDocument.Print();
            }
            return;
            // Викликаємо системний діалог друку для цього файлу
            //var p = new Process();
            var startInfo = new ProcessStartInfo
            {
                FileName = PdfFilePath,
                UseShellExecute = true,
                CreateNoWindow = true
            };
            try
            {
                // Спроба 1: Викликати прямий друк
                startInfo.Verb = "print";
                using (var p = Process.Start(startInfo))
                {
                    p?.WaitForExit(5000); 
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Спроба 2: Якщо "print" не підтримується (наприклад, стоїть Edge),
                // просто відкриваємо файл для перегляду.
                startInfo.Verb = ""; // Скидаємо Verb, щоб просто відкрити
                Process.Start(startInfo);
            }
        }

        private void OnClose(object obj)
        {
            // Логіка закриття (можна через інтерфейс IWindowService або Attached Property)
            (obj as dynamic)?.Close();
        }
    }
}