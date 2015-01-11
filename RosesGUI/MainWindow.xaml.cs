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
using System.Drawing;
using System.Text.RegularExpressions;
using System.Globalization;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String imageURI = @"pack://application:,,,/Images/roses.jpg";
        private static Bitmap resultImage = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            imageEnd.Source = Analize(imageURI);
            tabItem3.IsSelected = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                imageURI = dlg.FileName;
                label1.Content = imageURI;
                imageEnd.Source = Analize(imageURI);
            }
        }

        private BitmapSource Analize(String filename)
        {
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            imageStart.Source = src;
            imageStart.Stretch = Stretch.Fill;

            int hueMin = Int16.Parse(textBox1.Text);
            int hueMax = Int16.Parse(textBox2.Text);
            float saturationMin = Single.Parse(textBox3.Text, CultureInfo.InvariantCulture);
            float saturationMax = Single.Parse(textBox4.Text, CultureInfo.InvariantCulture);
            float luminanceMin = Single.Parse(textBox5.Text, CultureInfo.InvariantCulture);
            float luminanceMax = Single.Parse(textBox6.Text, CultureInfo.InvariantCulture);

            int blobWidth = Int16.Parse(textBox7.Text);
            int blobHeight = Int16.Parse(textBox8.Text);

            resultImage = Analizer.FindObjects(Analizer.BitmapImage2Bitmap(src), hueMin, hueMax, saturationMin, saturationMax, luminanceMin, luminanceMax, blobWidth, blobHeight);
            //Bitmap resultImage = Analizer.FindObjects(image);
            return Analizer.CreateBitmapSourceFromBitmap(resultImage);
        }

        private void textBox7_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
                e.Handled = isNumber(e.Text);
        }

        private bool isNumber(String input) {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(input);
        }

        private bool hasDotInString(String text) {
                return text.Contains(".");
        }

        private void textBox8_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isNumber(e.Text);
        }

        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isNumber(e.Text);
        }

        private void textBox2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = isNumber(e.Text);
        }

        private void textBox3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Equals(".") && textBox3.Text.Length > 0 && !hasDotInString(textBox3.Text))
            {
                e.Handled = false;
            }
            else {
                e.Handled = isNumber(e.Text);
            }
    
        }

        private void textBox4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Equals(".") && textBox3.Text.Length > 0 && !hasDotInString(textBox3.Text))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = isNumber(e.Text);
            }
        }

        private void textBox5_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Equals(".") && textBox3.Text.Length > 0 && !hasDotInString(textBox3.Text))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = isNumber(e.Text);
            }
        }

        private void textBox6_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Equals(".") && textBox3.Text.Length > 0 && !hasDotInString(textBox3.Text))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = isNumber(e.Text);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (resultImage != null) 
            {
                resultImage.Save(@"C:\merged.jpg");
            }
        }
    }
}
