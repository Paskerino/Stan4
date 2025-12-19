using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Stand4
{
    public class ConfirmationViewModel : BaseViewModel
    {
        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        // Цей Action ми викличемо, коли користувач натисне кнопку.
        // true - натиснув ОК, false - Скасувати.
        public Action<bool> OnResult { get; set; }

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }

        public ConfirmationViewModel(string message)
        {
            Message = message;

            YesCommand = new RelayCommand(_ =>
            {
                // Повертаємо true і кажемо "ми закінчили"
                OnResult?.Invoke(true);
            });

            NoCommand = new RelayCommand(_ =>
            {
                // Повертаємо false
                OnResult?.Invoke(false);
            });
        }
    }
}