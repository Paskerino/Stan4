using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stand4.ViewModels
{
    public class AppViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private object _currentPage;

        public object CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public AppViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ShowMainPage();
        }

        public void ShowMainPage()
        {
            // Отримуємо твою "стару" логіку з DI
            var mainPageVM = _serviceProvider.GetRequiredService<MainPageViewModel>();

            // Підписуємося на запит відкриття FTP
            //mainPageVM.RequestOpenFtp += () => {
               // CurrentPage = _serviceProvider.GetRequiredService<FtpViewModel>();
            //};

            CurrentPage = mainPageVM;
        }
    }
}
