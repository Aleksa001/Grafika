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
using Xceed.Wpf.Toolkit;

namespace Projekat1
{
    /// <summary>
    /// Interaction logic for EllipseDialog.xaml
    /// </summary>
    public partial class EllipseDialog : Window
    {
        public double EllipseWidth { get; set; }
        public double EllipseHeight { get; set; }
        public double StrokeTh { get; set; }
        public Brush EllipseStrokeColor { get; set; }
        public Brush TextColor { get; set; }
        public Brush Fill { get; set; }
        public string Text { get; set; }
        public double opacity { get; set; } 
        public EllipseDialog()
        {
            InitializeComponent();
            cmbColor.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbColor.SelectedIndex = 7;
            cmbTextColor.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbTextColor.SelectedIndex = 7;
            cmbFill.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbFill.SelectedIndex = 7;
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (double.TryParse(txtWidth.Text, out double width) &&
                double.TryParse(txtHeight.Text, out double height) && double.TryParse(txtStrokeTh.Text, out double strokeTh) && 
                double.TryParse(txtOpacity.Text, out double opac) && opac >= 0 && opac <= 1)
            {
                EllipseWidth = width;
                EllipseHeight = height;
                Text = txtInside.Text;
                DialogResult = true;
                StrokeTh = strokeTh;
                opacity = opac;
               
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid numbers for Width, Heigh, Thickness or Opacity!");
            }
           

            if (cmbColor.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)cmbColor.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                EllipseStrokeColor = brush;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid color!");
            }
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
           
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the dialog window without setting the properties
            DialogResult = false;
        }
    }
}
