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
    /// Interaction logic for TextDialog.xaml
    /// </summary>
    public partial class TextDialog : Window
    {
        public TextDialog()
        {
            InitializeComponent();
            TextColorComboBox.ItemsSource = new List<object>(typeof(Brushes).GetProperties()) { };
            TextColorComboBox.SelectedIndex = 7;
        }

        public string Text { get; set; }
        public Brush Color { get; set; }
        public double Size { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (TextColorComboBox.SelectedValue != null)
            {
                PropertyInfo x = (PropertyInfo)TextColorComboBox.SelectedItem;
                Brush brush = (Brush)x.GetValue(null);
                Color = brush;
            }
            else
            {
                System.Windows.MessageBox.Show("Please enter valid color!");
            }
            if (double.TryParse(SizetextBox.Text, out double size))
            {
                Size = size;
                Text = TextTextBox.Text;
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
