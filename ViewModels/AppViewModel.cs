using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stand4
{
    public class AppViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value;
                OnPropertyChanged(); }
        }

        public AppViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
           // GoToMainPage();
        }

        public void GoToMainPage()
        {
            var vm = _serviceProvider.GetRequiredService<MainPageViewModel>();
            // Підписуємося на подію переходу (якщо треба)
            CurrentView = vm;
        }

        public void RequestConfirmation(string message, Action<bool> callback)
        {
            // 1. Запам'ятовуємо, де ми були (опціонально, якщо хочеш повернутись точно туди ж)
            var previousView = CurrentView;

            // 2. Створюємо VM підтвердження
            var confirmVM = new ConfirmationViewModel(message);

            // 3. Налаштовуємо, що робити, коли там натиснуть кнопку
            confirmVM.OnResult = (result) =>
            {
                // Виконуємо логіку, яку просили (отримуємо інформацію result)
                callback(result);

                // 4. Повертаємося назад на головну (або на попередню)
                // GoToMainPage(); // Або:
                CurrentView = previousView;
            };

            // 5. Перемикаємо екран
            CurrentView = confirmVM;
        }
    }
}
