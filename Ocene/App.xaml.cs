using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Ocene
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    ///
    public partial class App : Application
    {
        public static SolidColorBrush zuta = new SolidColorBrush(Color.FromRgb(222, 239, 110));
        public static SolidColorBrush siva = new SolidColorBrush(Color.FromRgb(149, 150, 139));
        public static string baza = "";
        public static SolidColorBrush err = new SolidColorBrush(Color.FromRgb(255, 89, 89));
        public static bool setovanaGodina = false;
        public static int godina = 0;

        public static int Godina()
        {
            if (setovanaGodina) return godina;
            int yea = DateTime.Now.Year;
            int mon = DateTime.Now.Month;            
            if (mon >= 9) return yea;
            else return yea - 1;
        }
       
    }
}
