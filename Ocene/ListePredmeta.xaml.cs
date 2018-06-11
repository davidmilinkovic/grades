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
    /// Interaction logic for ListePredmeta.xaml
    /// </summary>
    /// 
    public partial class ListePredmeta : Window
    {
        ObservableCollection<Predmet> lista = new ObservableCollection<Predmet>();
        bool editMode = false;
        bool poziv = false;
        bool pomeraj = false;

        public ListePredmeta()
        {
            InitializeComponent();
            cmbSmer.ItemsSource = Smer.Daj();            
            lista = Smer.DajPredmete((Smer)cmbSmer.SelectedItem, cmbRazred.SelectedIndex + 1);
            lstPredmeti.ItemsSource = lista;
            
            lstPredmeti.ItemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(s_PreviewMouseLeftButtonDown)));
            lstPredmeti.ItemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(listbox1_Drop)));
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Predmeti pr = new Predmeti(true);
            pr.ShowDialog();
            if (pr.izabrani.Count != 0)
            {           
                foreach(Predmet x in pr.izabrani)
                {
                    x.prosek = true;
                    if(lista.Where(xx => xx.id == x.id).Count() == 0) lista.Add(x);
                }                     
                lstPredmeti.ItemsSource = lista;
            }            
        }
        

        private void cmbSmer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (poziv)
            {
                lista = Smer.DajPredmete((Smer)cmbSmer.SelectedItem, cmbRazred.SelectedIndex + 1);
                lstPredmeti.ItemsSource = lista;
            }
            poziv = true;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (lstPredmeti.SelectedIndex == -1) MessageBox.Show("Morate izabrati predmet sa liste da biste ga obrisali", "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                lista.Remove((Predmet)lstPredmeti.SelectedItem);
                lstPredmeti.ItemsSource = lista;
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            odabirGrid.Visibility = Visibility.Collapsed;
            editGrid.Visibility = Visibility.Visible;
            lstPredmeti.IsEnabled = true;
            btnRed.Visibility = Visibility.Visible;
            lstPredmeti.Background = Brushes.LightYellow;
            editMode = true;
        }

        private void btnCanc_Click(object sender, RoutedEventArgs e)
        {
            cmbSmer_SelectionChanged(null, null);
            odabirGrid.Visibility = Visibility.Visible;
            editGrid.Visibility = Visibility.Collapsed;
            lstPredmeti.IsEnabled = false;
            btnRed.Visibility = Visibility.Hidden;
            lstPredmeti.Background = Brushes.White;
            editMode = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            odabirGrid.Visibility = Visibility.Visible;
            editGrid.Visibility = Visibility.Collapsed;
            btnRed.Visibility = Visibility.Hidden;
            lstPredmeti.IsEnabled = false;
            Smer.SacuvajListuPredmeta(lista, (Smer)cmbSmer.SelectedItem, cmbRazred.SelectedIndex + 1);
            lstPredmeti.Background = Brushes.White;
            editMode = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(editMode)
            {
                MessageBoxResult res = MessageBox.Show("Da li zelite da sacuvate izmene koje ste napravili spisku?", "Pitanje", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes) btnSave_Click(null, null);
                else if (res == MessageBoxResult.Cancel) e.Cancel = true; 
            }
        }
        void s_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (pomeraj)
            {
                if (sender is ListBoxItem)
                {                    
                    ListBoxItem draggedItem = sender as ListBoxItem;
                    DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }
        }

        void listbox1_Drop(object sender, DragEventArgs e)
        {
            if (pomeraj)
            {                
                Predmet droppedData = e.Data.GetData(typeof(Predmet)) as Predmet;
                Predmet target = ((ListBoxItem)(sender)).DataContext as Predmet;

                int removedIdx = lstPredmeti.Items.IndexOf(droppedData);
                int targetIdx = lstPredmeti.Items.IndexOf(target);

                if (removedIdx < targetIdx)
                {
                    lista.Insert(targetIdx + 1, droppedData);
                    lista.RemoveAt(removedIdx);
                }
                else
                {
                    int remIdx = removedIdx + 1;
                    if (lista.Count + 1 > remIdx)
                    {
                        lista.Insert(targetIdx, droppedData);
                        lista.RemoveAt(remIdx);
                    }
                }
            }
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            if(btnRed.Content.ToString() == "OK")
            { pomeraj = false; btnRed.Content = "Izmeni redosled"; }
            else
            { pomeraj = true; btnRed.Content = "OK"; }
        }
    }
}
