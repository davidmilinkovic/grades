using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for Predmeti.xaml
    /// </summary>
    public partial class Predmeti : Window
    {
        ObservableCollection<Predmet> lista;
        public List<Predmet> izabrani = new List<Predmet>();
        int mode = 0; // 0 - standard, 1 - edit, 2 - add
        bool b1 = false;
        // -1 - obican, -2 - tip izbornog, id - izborni

        public Predmeti(bool modIzbora)
        {
            InitializeComponent();
            lista = Predmet.Daj("tip = -1");
            lstPredmeti.Focus();
            lstPredmeti.ItemsSource = lista;
            if (modIzbora) ModIzbora();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int cnt = lista.Count();
            Predmet pr = new Predmet() { tip = -1 };
            if (cnt != 0) pr.id = Predmet.Daj().Last().id + 1;
            else pr.id = 1;
            lista.Add(pr);
            lstPredmeti.SelectedIndex = cnt;
            EditMode(false);
            mode = 2;
            txtPredmet.Focus();
        }

        void EditMode(bool rev)
        {
            if (rev)
            {
                g2.Visibility = Visibility.Hidden;
                g1.Visibility = Visibility.Visible;
                txtPredmet.IsReadOnly = true;
                lstPredmeti.ItemsSource = lista;
                lstPredmeti.SelectedIndex = 0;
                txtPredmet.Background = Brushes.White;
                gTip.IsEnabled = true;
                lstPredmeti.IsEnabled = true;
            }
            else
            {
                g2.Visibility = Visibility.Visible;
                g1.Visibility = Visibility.Hidden;
                txtPredmet.IsReadOnly = false;
                txtPredmet.Focus();
                txtPredmet.Background = App.zuta;
                gTip.IsEnabled = false;
                lstPredmeti.Focus();
                lstPredmeti.ScrollIntoView(lstPredmeti.SelectedItem);
                lstPredmeti.IsEnabled = false;
            }
        }

        private void btnEdi_Click(object sender, RoutedEventArgs e)
        {
            if (lstPredmeti.SelectedIndex == -1)
            {
                MessageBox.Show("Morate da izaberete predmet!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                EditMode(false);
                mode = 1;
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (lstPredmeti.SelectedIndex == -1)
            {
                MessageBox.Show("Morate da izaberete predmet!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Predmet pr = (Predmet)lstPredmeti.SelectedItem;
                MessageBoxResult res = MessageBox.Show("Da li ste sigurni da zelite da izbrisete predmet " + pr.naziv + "?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    Predmet.Izbrisi(pr);
                    ((ObservableCollection<Predmet>)lstPredmeti.ItemsSource).Remove(pr);
                    lstPredmeti.ItemsSource = lista;
                    lstPredmeti.SelectedIndex = 0;
                }
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Predmet pr = (Predmet)lstPredmeti.SelectedItem;
            if (cmbVrsta.SelectedIndex < 2) pr.tip = -1 * (cmbVrsta.SelectedIndex + 1);
            else pr.tip = ((Predmet)cmbPodvrsta.SelectedItem).id;

            if (mode == 1) Predmet.Izmeni(pr);
            else if (mode == 2) Predmet.Dodaj(pr);

            mode = 0;
            EditMode(true);
        }

        private void btnCanc_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 1) cmbVrsta_SelectionChanged(null, null);
            else if (mode == 2) lista.Remove(lista.Last());
            mode = 0;
            EditMode(true);
        }

        void ModIzbora()
        {
            g1.Visibility = Visibility.Hidden;
            g3.Visibility = Visibility.Visible;
            txtPredmet.Visibility = Visibility.Collapsed;
            lista = Predmet.Daj("tip < 0");
            lstPredmeti.ItemsSource = lista;
            lstPredmeti.Focus();
            lstPredmeti.SelectionMode = SelectionMode.Extended;
        }

        private void btnIzab_Click(object sender, RoutedEventArgs e)
        {
            if (lstPredmeti.SelectedIndex == -1) MessageBox.Show("Morate izabrati predmet", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                izabrani.Clear();
                foreach (var x in lstPredmeti.SelectedItems) izabrani.Add(x as Predmet);
                this.Close();
            }
        }

        private void btnIzabC_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbVrsta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (b1)
            {
                Predmet pr = (Predmet)lstPredmeti.SelectedItem;
                if (cmbVrsta.SelectedIndex == 2)
                {
                    List<Predmet> lPred = Predmet.Daj().Where(x => x.tip == -2).ToList();
                    cmbPodvrsta.Visibility = Visibility.Visible;
                    cmbPodvrsta.ItemsSource = lPred;
                    if (lPred.Count != 0)
                    {
                        cmbPodvrsta.SelectedIndex = 0;
                        lista = Predmet.Daj("tip = " + lPred[0].id.ToString());
                        lstPredmeti.ItemsSource = lista;
                    }
                }
                else
                {
                    cmbPodvrsta.Visibility = Visibility.Hidden;
                    lista = Predmet.Daj("tip = " + ((cmbVrsta.SelectedIndex + 1) * -1).ToString());
                    lstPredmeti.ItemsSource = lista;
                }
            }
            b1 = true;
        }

        private void lstPredmeti_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void cmbPodvrsta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                lista = Predmet.Daj("tip = " + ((Predmet)cmbPodvrsta.SelectedItem).id.ToString());
                lstPredmeti.ItemsSource = lista;
            }
            catch
            {
                lista.Clear();
            }
        }




    }
}