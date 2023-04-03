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
    /// Interaction logic for ChangeEllipseDialog.xaml
    /// </summary>
    public partial class ChangeEllipseDialog : Window
    {
        public Brush StrokeColor { get; set; }   
        public Brush Fill { get; set; }   
        public double StrokeTh { get; set; }
        public ChangeEllipseDialog()
        {
            InitializeComponent();
            cmbColor.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbColor.SelectedIndex = 7;
            cmbFill.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbFill.SelectedIndex = 7;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbColor.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)cmbColor.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                StrokeColor = brush;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid color!");
            }
            if (cmbFill.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)cmbFill.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                Fill = brush;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid color!");
            }
            if ( double.TryParse(txtStrokeTh.Text, out double strokeTh))
            {
               
                StrokeTh = strokeTh;
                DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid numbers for Thickness!");
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
