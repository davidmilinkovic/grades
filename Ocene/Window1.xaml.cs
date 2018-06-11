using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ocene
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            StreamReader sr = new StreamReader("baza.txt");
            App.baza = sr.ReadLine();
            sr.Close();

            int goidna = App.Godina();
            for (int i = 0; i < 10; i++) cmbGodina.Items.Add(new ComboBoxItem() { Content = (goidna - i).ToString() + "/" + ((goidna - i + 1) % 100).ToString() });
            cmbGodina.SelectedIndex = 0;            
                       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Ucenici win = new Ucenici();
            win.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Predmeti win = new Predmeti(false);
            win.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Smerovi win = new Smerovi();
            win.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ListePredmeta lst = new ListePredmeta();
            lst.Show();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            OceneWin oc = new Ocene.OceneWin();
            oc.Show();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            Stampanje win = new Stampanje();
            win.Show();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            Podesavanja pod = new Podesavanja();
            pod.Show();
        }

        private void cmbGodina_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGodina.SelectedIndex != -1)
            {
                string s = cmbGodina.SelectedItem.ToString();
                s = s.Substring(s.IndexOf(" ") + 1, 4);
                App.setovanaGodina = true;
                App.godina = Convert.ToInt32(s);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }
    }
}
