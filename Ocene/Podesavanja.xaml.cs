using System;
using System.Collections.Generic;
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
    /// Interaction logic for Podesavanja.xaml
    /// </summary>
    public partial class Podesavanja : Window
    {
        public Podesavanja()
        {
            InitializeComponent();
            lstSettings.ItemsSource = Podesavanje.Daj();            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Podesavanje.Sacuvaj((List<Podesavanje>)lstSettings.ItemsSource);
        }
    }
}
