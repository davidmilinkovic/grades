using System;
using System.Collections.Generic;
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
    /// Interaction logic for Stampanje.xaml
    /// </summary>
    public partial class Stampanje : Window
    {
        public Stampanje()
        {
            InitializeComponent();
            cmbRazred.ItemsSource = Ucenik.DajRazrede();            
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
                int raz = Convert.ToInt32(cmbRazred.SelectedValue);
                int ode = Convert.ToInt32(cmbOdeljenje.SelectedValue);

                lb.ItemsSource = Ucenik.Daj().Where(u => u.razred == raz && u.odeljenje == ode).ToList();                
                lb.Focus();
            }
        }

        private BackgroundWorker worker;
        private Ucitavanje dialog;
        string putanja = "";
        private List<Ucenik> lista = new List<Ucenik>();

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (putanja != "")
            {
                dialog = new Ucitavanje();
                lista.Clear();
                foreach (var v in lb.SelectedItems) lista.Add(v as Ucenik);
                if (lista.Count > 0)
                {
                    worker = new BackgroundWorker();
                    worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                    worker.RunWorkerAsync();

                    this.Dispatcher.BeginInvoke(new Action(() => dialog.ShowDialog()));
                }
                else MessageBox.Show("Morate izabrati ucenike za stampu!");
            }
            else MessageBox.Show("Morate izabrati putanju za snimanje fajlova!");
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => dialog.Close()));
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Stampa st = new Stampa(putanja);
            st.init();
            string prvi = putanja;
            foreach (Ucenik uce in lista)
            {                
                st.StampajZaUcenika(uce);
                MessageBox.Show(uce.naziv);
            }
            st.kraj();
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            if (dialog.SelectedPath != string.Empty) putanja = dialog.SelectedPath;
            txtPath.Text = putanja;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lb.SelectAll();
            lb.Focus();
        }
    }
}
