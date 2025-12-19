using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stand4
{
    /// <summary>
    /// Interaction logic for testUC.xaml
    /// </summary>
    public partial class testUC : UserControl
    {
    }

    // Допоміжний клас для роботи з системними функціями
    public static class SystemServiceTEST
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime(ref SYSTEMTIME st);

        public static void SetSystemTime(DateTime newDateTime)
        {
            DateTime universalTime = newDateTime.ToUniversalTime();
            SYSTEMTIME st = new SYSTEMTIME
            {
                wYear = (short)universalTime.Year,
                wMonth = (short)universalTime.Month,
                wDay = (short)universalTime.Day,
                wHour = (short)universalTime.Hour,
                wMinute = (short)universalTime.Minute,
                wSecond = (short)universalTime.Second,
                wMilliseconds = (short)universalTime.Millisecond
            };

            if (!SetSystemTime(ref st))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
        }
    }
}