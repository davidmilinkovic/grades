using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Ocene
{
    /// <summary>
    /// Interaction logic for Smerovi.xaml
    /// </summary>
    public partial class Smerovi : Window
    {
        ObservableCollection<Smer> lista;
        int mode = 0; // 0 - standard, 1 - edit, 2 - add

        public Smerovi()
        {
            InitializeComponent();
            lista = Smer.Daj();
            lstSmerovi.ItemsSource = lista;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int cnt = lista.Count();
            Smer pr = new Smer();
            if (cnt != 0) pr.id = ((ObservableCollection<Smer>)lstSmerovi.ItemsSource).Last().id + 1;
            else pr.id = 1;
            ((ObservableCollection<Smer>)lstSmerovi.ItemsSource).Add(pr);
            lstSmerovi.SelectedIndex = cnt;
            EditMode(false);
            mode = 2;
        }

        void EditMode(bool rev)
        {
            if (rev)
            {
                g2.Visibility = Visibility.Hidden;
                g1.Visibility = Visibility.Visible;
                txtSmer.IsReadOnly = true;
                lstSmerovi.ItemsSource = lista;
                lstSmerovi.SelectedIndex = 0;
                txtSmer.Background = Brushes.White;
            }
            else
            {
                g2.Visibility = Visibility.Visible;
                g1.Visibility = Visibility.Hidden;
                txtSmer.IsReadOnly = false;

                txtSmer.Background = App.zuta;
            }
        }

        private void btnEdi_Click(object sender, RoutedEventArgs e)
        {
            EditMode(false);
            mode = 1;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {

            Smer pr = (Smer)lstSmerovi.SelectedItem;
            MessageBoxResult res = MessageBox.Show("Da li ste sigurni da zelite da izbrisete predmet " + pr.naziv + "?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                Smer.Izbrisi(pr);
                ((ObservableCollection<Smer>)lstSmerovi.ItemsSource).Remove(pr);
                lstSmerovi.ItemsSource = lista;
                lstSmerovi.SelectedIndex = 0;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Smer pr = (Smer)lstSmerovi.SelectedItem;
            if (mode == 1) Smer.Izmeni(pr);
            else if (mode == 2) Smer.Dodaj(pr);

            mode = 0;
            EditMode(true);
        }

        private void btnCanc_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 1) lista = Smer.Daj();
            else if (mode == 2) lista.Remove(lista.Last());
            mode = 0;
            EditMode(true);
        }
    }
}
