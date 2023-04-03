using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projekat1
{
    /// <summary>
    /// Interaction logic for ChangeTextDialog.xaml
    /// </summary>
    public partial class ChangeTextDialog : Window
    {
        public Brush TextColor { get; set; }
        public double textSize { get; set; }

        public ChangeTextDialog()
        {
            InitializeComponent();
            cmbTextColor.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbTextColor.SelectedIndex = 7;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTextColor.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)cmbTextColor.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                TextColor = brush;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid color!");
            }
            if (double.TryParse(txtSize.Text, out double sizet) )
            {
                textSize = sizet;
                DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid numbers for Text size!");
            }
           
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
