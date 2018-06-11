using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
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
    /// Interaction logic for Ocene.xaml
    /// </summary>
    public partial class OceneWin : Window
    {
        Smer cursmer = new Smer();
        DataTemplate ocenaTmp = new DataTemplate();
        List<Predmet> trenutniPredmeti = new List<Predmet>();
        List<Ucenik> trenutniUcenici = new List<Ucenik>();
        DataTable trenutniSors = new DataTable();
        GridViewColumn prva;

        public OceneWin()
        {
            InitializeComponent();
            cmbRazred.ItemsSource = Ucenik.DajRazrede();
            prva = ((GridView)lvv.View).Columns[0];
            txtGod.Text = App.Godina().ToString();
        }


        private void cmbRazred_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbOdeljenje.ItemsSource = Ucenik.DajOdeljenja(Convert.ToInt32(cmbRazred.SelectedValue));
            if (cmbOdeljenje.SelectedIndex == -1) wrOdelj.Visibility = Visibility.Visible;
            else cmbOdeljenje.SelectedIndex = -1;
        }

        private void cmbOdeljenje_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbOdeljenje.SelectedIndex != -1)
            {
                for (int i = 0; i < trenutniPredmeti.Count; i++) ((GridView)lvv.View).Columns.RemoveAt(1);

                int raz = Convert.ToInt32(cmbRazred.SelectedValue);
                int ode = Convert.ToInt32(cmbOdeljenje.SelectedValue);
                trenutniUcenici = Ucenik.Daj().Where(u => u.razred == raz && u.odeljenje == ode).ToList();
                cursmer = trenutniUcenici[0].smer;
                lbSmer.Content = cursmer.naziv;
                trenutniPredmeti = Smer.DajPredmete(cursmer, raz).ToList();

                DataTable sors = new DataTable();
                sors.Columns.Add("ime", typeof(string));

                foreach (Predmet pr in trenutniPredmeti)
                {
                    string x = pr.naziv.Trim().Replace(".", "");
                    sors.Columns.Add(x, typeof(string));

                    GridViewColumn col = new GridViewColumn();
                    col.Header = pr.naziv;

                    DataTemplate temp = new DataTemplate();

                    FrameworkElementFactory bor = new FrameworkElementFactory(typeof(Border));
                    bor.SetValue(Border.BorderBrushProperty, Brushes.LightGray);
                    bor.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 1, 1));
                    bor.SetValue(Border.MarginProperty, new Thickness(-6, 0, -6, 0));

                    RoutedEventHandler izgubioFokus = Fokus;
                    RoutedEventHandler dobioFokus = DFokus;

                    FrameworkElementFactory title = new FrameworkElementFactory(typeof(TextBox));
                    title.SetValue(TextBox.FontWeightProperty, FontWeights.Bold);
                    title.SetBinding(TextBox.TextProperty, new Binding(x));
                    title.SetValue(TextBox.MarginProperty, new Thickness(5));
                    title.AddHandler(TextBox.LostFocusEvent, izgubioFokus);
                    title.AddHandler(TextBox.GotFocusEvent, dobioFokus);
                    title.SetValue(TextBox.WidthProperty, (double)23);
                    title.SetValue(TextBox.TabIndexProperty, 1);
                    title.SetValue(TextBox.IsTabStopProperty, true);

                    bor.AppendChild(title);
                    temp.VisualTree = bor;
                    col.CellTemplate = temp;

                    ((GridView)lvv.View).Columns.Add(col);
                }

                foreach (Ucenik uc in trenutniUcenici)
                {
                    DataRow row = sors.NewRow();
                    row[0] = uc.naziv;
                    int i = 1;
                    foreach (Predmet pr in trenutniPredmeti)
                    {
                        int? ocena = Ucenik.OcenaIz((int)uc.broj, pr.id, App.Godina()); ;
                        if (ocena == null) row[i] = "";
                        else row[i] = ocena.ToString();
                        i++;
                    }
                    sors.Rows.Add(row);
                }

                trenutniSors = sors.Copy();
                lvv.ItemsSource = sors.DefaultView;
                ((GridView)lvv.View).Columns[0].Width = 200;
            }
        }
        void Fokus(object sender, RoutedEventArgs e)
        {
            TextBox boks = ((TextBox)sender);
            if (boks.Text == "" || boks.Text == null) return;

            string txt = boks.Text;
            int n;
            if (int.TryParse(txt, out n))
            {
                if (n < 1 || n > 5)
                {
                    MessageBox.Show("Ocena nije validna");
                    Dispatcher.BeginInvoke((ThreadStart)delegate { boks.Focus(); });
                    boks.SelectAll();
                }
            }
            else
            {
                MessageBox.Show("Ocena nije validna");
                Dispatcher.BeginInvoke((ThreadStart)delegate { boks.Focus(); });
                boks.SelectAll();
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int raz = Convert.ToInt32(cmbRazred.SelectedValue);
            int ode = Convert.ToInt32(cmbOdeljenje.SelectedValue);
            DataTable sors = ((DataView)lvv.ItemsSource).Table;
            int u = 0;
            SQLiteConnection con = new SQLiteConnection("Data Source = " + App.baza);
            con.Open();
            int c = 0;
            SQLiteCommand com;
            foreach (Ucenik uce in trenutniUcenici)
            {
                var red = sors.Rows[u].ItemArray;
                for (int i = 1; i < red.Length; i++)
                {
                    if (red[i] != trenutniSors.Rows[u].ItemArray[i])
                    {
                        com = new SQLiteCommand(String.Format("DELETE FROM ocene WHERE id_ucenika = {0} AND id_predmeta = {1} AND godina = {2}", uce.broj, trenutniPredmeti[i - 1].id, App.Godina()), con);
                        com.ExecuteNonQuery();
                        if (red[i] != null && red[i] != DBNull.Value && red[i].ToString() != "" && red[i].ToString() != string.Empty)
                        {
                            com = new SQLiteCommand(String.Format("INSERT INTO ocene VALUES ({0},{1},{2},{3})", uce.broj, trenutniPredmeti[i - 1].id, red[i].ToString(), App.Godina()), con);
                            com.ExecuteNonQuery();
                        }
                        c++;
                    }
                }
                u++;
            }
            con.Close();
            trenutniSors = sors.Copy();
        }

        private void btnCanc_Click(object sender, RoutedEventArgs e)
        {
            lvv.ItemsSource = trenutniSors.DefaultView;
        }

        private void DFokus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.setovanaGodina = true;
            App.godina = Convert.ToInt32(txtGod.Text);
            cmbOdeljenje_SelectionChanged(null, null);
        }
    }
}
