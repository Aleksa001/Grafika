using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projekat1
{
    /// <summary>
    /// Interaction logic for PolygonDialog.xaml
    /// </summary>
    public partial class PolygonDialog : Window
    {
        public PolygonDialog()
        {
            InitializeComponent();
            PolygonStrokeColorComboBox.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            PolygonFillComboBox.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            PolygonStrokeColorComboBox.SelectedIndex = 7;
            PolygonFillComboBox.SelectedIndex = 7;
            cmbTextColor.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            cmbTextColor.SelectedIndex = 7;
        }
        
        public Brush PolygonFill { get; set; }
        public double PolygonStrokeThickness { get; set; }
        public Brush PolygonStrokeColor { get; set; }
        public Brush TextColor { get; set; }
        public string Text { get; set; }
        public double opacity { get; set; }

        private void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            if (PolygonStrokeColorComboBox.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)PolygonStrokeColorComboBox.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                PolygonStrokeColor = brush;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid color!");
            }
            if (PolygonFillComboBox.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)PolygonFillComboBox.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                PolygonFill = brush;
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
            if (double.TryParse(txtOpacity.Text, out double opac) && opac >= 0 && opac <= 1 && double.TryParse(PolygonStrokeThicknessTextBox.Text, out double strokeTh))
            {
                opacity = opac;
                PolygonStrokeThickness = strokeTh;
                Text = txtInside.Text;
                DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid numbers for Opacity and Thickness!");
            }
           
        
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
