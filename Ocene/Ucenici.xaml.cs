using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Ucenici.xaml
    /// </summary>
    public partial class Ucenici : Window
    {
        List<TextBox> boksovi = new List<TextBox>();        
        ObservableCollection<Ucenik> lista = new ObservableCollection<Ucenik>();
        int stariId = 0;        
        int mode = 0; // 0 - standard, 1 - edit, 2 - add

        public Ucenici()
        {
            InitializeComponent();

            lista = Ucenik.Daj();
            lb_ucenici.ItemsSource = lista;
            memberCmb.ItemsSource = Ucenik.DajPolja(true);
            memberCmb.SelectedIndex = 3;
            txt11.ItemsSource = Smer.Daj();
            boksovi.Add(txt1);
            boksovi.Add(txt2);
            boksovi.Add(txt3);
            boksovi.Add(txt4);
            boksovi.Add(txt5);
            boksovi.Add(txt6);
            boksovi.Add(txt7);
            boksovi.Add(txt8);
            boksovi.Add(txt9);
            boksovi.Add(txt12);
            boksovi.Add(txt14);
            EditMode(true);
            Grupisanje();            
        }

        private void Grupisanje()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lb_ucenici.ItemsSource);
            view.GroupDescriptions.Add(new PropertyGroupDescription("sraz"));
            view.GroupDescriptions.Add(new PropertyGroupDescription("razod"));
            view.Refresh();
        }

        private void RGrupisanje()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lb_ucenici.ItemsSource);            
            view.Refresh();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            lb_ucenici.ItemsSource = lista;
            int cnt = lista.Count();
            Ucenik uc = new Ucenik() { redovan = 1 };
            ((ObservableCollection<Ucenik>)lb_ucenici.ItemsSource).Add(uc);
            lb_ucenici.SelectedIndex = cnt;
            EditMode(false);
            mode = 2;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if(lb_ucenici.SelectedIndex == -1)
            {
                MessageBox.Show("Morate izabrati ucenika!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
            EditMode(false);
            stariId = Convert.ToInt32(txt1.Text);
            mode = 1;            
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (lb_ucenici.SelectedIndex == -1)
            {
                MessageBox.Show("Morate izabrati ucenika!", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
            Ucenik uc = (Ucenik)lb_ucenici.SelectedItem;
            MessageBoxResult res = MessageBox.Show("Da li ste sigurni da zelite da izbrisete ucenika " + uc.naziv + " sa brojem "+uc.broj.ToString()+"?", "Pitanje", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                Ucenik.Izbrisi(uc);
                lista.Remove(uc);
                lb_ucenici.ItemsSource = lista;
                lb_ucenici.SelectedIndex = 0;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBoxResult.Yes;
            foreach(TextBox bb in boksovi)
            {
                if(bb.Background == App.err)
                {
                    res = MessageBox.Show("Postoje greske u nekim poljima (oznacena crveno)! Da li zelite da nastavite?", "Greske", MessageBoxButton.YesNo);                    
                }
            }
            if (res == MessageBoxResult.Yes)
            {
                Ucenik uc = (Ucenik)lb_ucenici.SelectedItem;
                uc.smer = (Smer)txt11.SelectedItem;
                if (mode == 1) Ucenik.Izmeni(uc, stariId);
                else if (mode == 2) Ucenik.Dodaj(uc);
                mode = 0;
                EditMode(true);
                RGrupisanje();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            int olm = mode;
            mode = 0;
            if (olm == 1) lista = Ucenik.Daj();
            else if (olm == 2) lista.Remove(lista.Last());            
            EditMode(true);
        }

        void EditMode(bool rev)
        {
            if (rev)
            {
                lb_ucenici.IsEnabled = true;
                wrap2.Visibility = Visibility.Hidden;
                wrap1.Visibility = Visibility.Visible;
                foreach (var a in boksovi) a.IsReadOnly = true;
                txt10.IsEnabled = false;
                txt11.IsEnabled = false;
                txt13.IsEnabled = false;
                lb_ucenici.ItemsSource = lista;                
                lb_ucenici.SelectedIndex = 0;
                txtSearch.IsEnabled = true;
                txtSearch.Text = "";
                foreach (var a in boksovi) a.Background = Brushes.White;
                txt10.Background = Brushes.White;
                txt11.Background = Brushes.White;
                txt12.Background = Brushes.White;
            }
            else
            {
                lb_ucenici.IsEnabled = false;
                wrap2.Visibility = Visibility.Visible;
                wrap1.Visibility = Visibility.Hidden;
                foreach (var a in boksovi) a.IsReadOnly = false;

                txt10.IsEnabled = true;
                txt11.IsEnabled = true;
                txt13.IsEnabled = true;

                txt10.Background = App.zuta;
                txt11.Background = App.zuta;
                txt13.Background = App.zuta;
                
                txtSearch.IsEnabled = false;
                foreach (var a in boksovi) a.Background = App.zuta;
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lb_ucenici.ItemsSource = lista.Where(x => GetPropValue(x, memberCmb.SelectedValue.ToString()).ToString().IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            lb_ucenici.SelectedIndex = 0;
            if(txtSearch.Text == "") Grupisanje();
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        private void memberCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtSearch.Text = "";
        }

        string pre = "";

        private void txt1_GotFocus(object sender, RoutedEventArgs e)
        {
            pre = txt1.Text;
        }

        private void txt1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pre != txt1.Text)
            {                
                MBroj(Ucenik.ValidateBroj(txt1.Text));
            }
        }

        private void txt3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txt3.Text.Length >= 7) GenDate(txt3.Text);

            Jmbg(Ucenik.ValidateJmbg(txt3.Text));            
        }

        private void GenDate(string date)
        {
            string datum = date.Substring(0, 2) + "." + date.Substring(2, 2) + ".";
            int yea = int.Parse(date.Substring(4, 3));
            if (yea > 500) datum += "1" + date.Substring(4, 3) + ".";
            else datum += "2" + date.Substring(4, 3) + ".";
            txt6.Text = datum;
        }

        private void Jmbg(bool ok)
        {
            if(!ok)
            {
                txt3.Background = App.err;
                txt3.ToolTip = "Uneti JMBG nije validan.";
            }
            else
            {
                txt3.Background = txt2.Background;
                txt3.ToolTip = "";
            }
        }

        private void MBroj(bool ok)
        {
            if (!ok)
            {
                txt1.Background = App.err;
                txt1.ToolTip = "Uneti maticni broj nije validan.";
            }
            else
            {                
                txt1.Background = txt2.Background;
                txt3.ToolTip = "";
                Ucenik tren = lb_ucenici.SelectedItem as Ucenik;
                tren.sbroj = txt1.Text;
                tren.broj = Convert.ToInt32(tren.sbroj);                
                var istiSmer = Ucenik.Daj().Where(x => x.god_upisa == tren.god_upisa && x.odeljenje == tren.odeljenje).ToList();
                if(istiSmer.Count != 0) txt11.SelectedIndex = istiSmer[0].smer.id - 1;
            }

        }
    }
}
