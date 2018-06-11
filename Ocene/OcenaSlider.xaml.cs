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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ocene
{
    /// <summary>
    /// Interaction logic for OcenaSlider.xaml
    /// </summary>
    public partial class OcenaSlider : UserControl
    {
        private int _numValue = 0;

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }

        public OcenaSlider(int? num)
        {
            InitializeComponent();
            txtNum.Text = _numValue.ToString();
            if (num != null)
            {
                NumValue = (int)num;
            }
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _numValue))
            {
                if (_numValue > 5 || _numValue < 1) _numValue = 5;
                txtNum.Text = _numValue.ToString();
            }
              
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
