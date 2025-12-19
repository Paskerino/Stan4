using CommonLogic.BusinessLogic.Interfaces;
using CommonLogic.BusinessLogic.Managers;
using CommonLogic.Core.DAL.Interfaces;
using CommonLogic.Core.Models;
using CommonLogic.DAL.Implementation;
using CommonLogic.Logic.Services.Interfaces;
using CommonLogic.Services.Implementations;
using CommonLogic.Services.Interfaces;
using CommonLogic.Services.Workers;
using Microsoft.Extensions.DependencyInjection;
using Stand4.Services;
using Stand4.UC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Stand4
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            //InitializeComponent();
            // 3. Створюємо колекцію сервісів
            var services = new ServiceCollection();

            // 4. Налаштовуємо (реєструємо) всі наші класи
            ConfigureServices(services);

            // 5. Збираємо фінальний провайдер сервісів
            _serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            // --- Крок 0: Конфігурація ---
            // Завантажуємо конфіги один раз і реєструємо їх як 'singleton'
            var configLoader = new ConfigurationLoader();
            List<Device> allDevices = configLoader.LoadDevices();
            List<TriggerRule> allRules = configLoader.LoadTriggerRules();

            // Реєструємо самі списки, щоб інші сервіси могли їх отримати
            // (Контейнер знатиме, що якщо хтось просить List<Device>, треба віддати цей екземпляр)
            services.AddSingleton(allDevices);
            services.AddSingleton(allRules);

            // --- Крок 1: Реєстрація Сервісів ---
            // (Singleton - один екземпляр на весь додаток)
            services.AddSingleton<IReadingRepository, FileReadingRepository>();
            services.AddSingleton<IModbusService, ModbusService>();
            services.AddSingleton<IModbusPollingService, ModbusPollingService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<ITestExecutionService, TestExecutionService>();
            services.AddSingleton<IDataLoggerService, CsvLoggerService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<ILogReaderService, CsvLogReaderService>();
            services.AddSingleton<IStatusLineParsingService, StatusLineParsingService>();
           



            // --- Крок 2: Реєстрація Бізнес-логіки ---
            // Контейнер автоматично "зрозуміє", що для IDataManager 
            // потрібні IModbusService, IReadingRepository і т.д. і сам їх підставить.
            services.AddSingleton<IDataManager, DataManager>();

            // --- Крок 3: Реєстрація ViewModel ---
            // Так само, він автоматично впровадить IDataManager і IModbusPollingService
            // у конструктор MainViewModel.
            //services.AddSingleton<MainViewModel>();

            //// --- Крок 4: Реєстрація View ---
            //// Реєструємо наше головне вікно
            //services.AddSingleton<MainWindow>();


            services.AddSingleton<AppViewModel>();

            // 2. Твою поточну MainViewModel реєструємо як MainPageViewModel 
            // (це та сама логіка, просто тепер це "одна зі сторінок")
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<ConfirmationViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainTemplate = new DataTemplate(typeof(MainPageViewModel));
            mainTemplate.VisualTree = new FrameworkElementFactory(typeof(MainView));
            this.Resources.Add(new DataTemplateKey(typeof(MainPageViewModel)), mainTemplate);
            //Test
             var testTemplate = new DataTemplate(typeof(ConfirmationViewModel));
            testTemplate.VisualTree = new FrameworkElementFactory(typeof(ConfirmationView));
             this.Resources.Add(new DataTemplateKey(typeof(ConfirmationViewModel)), testTemplate);

            var appViewModel = _serviceProvider.GetRequiredService<AppViewModel>();
            appViewModel.GoToMainPage();

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}

