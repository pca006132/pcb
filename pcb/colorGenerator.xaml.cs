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
using System.Text.RegularExpressions;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace pcb
{
    /// <summary>
    /// Interaction logic for colorGenerator.xaml
    /// </summary>
    public partial class colorGenerator
    {
        public string text;
        public colorGenerator()
        {
            InitializeComponent();
        }
        private string getColorNBTValue(Color color)
        {
            return ((color.R << 16) + (color.G << 8) + color.B).ToString();
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SolidColorBrush colorbrush = new SolidColorBrush();
            colorbrush.Color = Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value);
            rectangle.Fill = colorbrush;
            ColorHex.Text = HexConverter(colorbrush.Color);
        }
        private static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        private void btnChangeColor_Click(object sender, RoutedEventArgs e)
        {
            ColorHex.Text = ColorHex.Text.ToLower();
            if (Regex.IsMatch(ColorHex.Text, @"^#[0-9a-f]{6}$"))
            {
                try
                {
                    SolidColorBrush colorbrush = new SolidColorBrush();
                    colorbrush.Color = (Color)ColorConverter.ConvertFromString(ColorHex.Text);
                    rectangle.Fill = colorbrush;
                    sliderR.Value = colorbrush.Color.R;
                    sliderG.Value = colorbrush.Color.G;
                    sliderB.Value = colorbrush.Color.B;
                }
                catch
                {
                    CustomMessageBox.ShowMessage("wrong input", "error", false);
                }
            }
            else
            {
                CustomMessageBox.ShowMessage("format should be: \"#xxxxxx\"(hex)", "error", false);
            }
        }
        private void btnAddColor_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value);
            if (colorArray.Text == "")
                colorArray.AppendText(getColorNBTValue(color));
            else
                colorArray.AppendText("," + getColorNBTValue(color));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (colorArray.Text == "")
            {
                Color color = Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value);
                colorArray.AppendText(getColorNBTValue(color));
            }
            text = colorArray.Text;
            Hide();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}