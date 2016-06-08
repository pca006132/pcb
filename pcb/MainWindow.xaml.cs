using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using pcb.core;
using pcb.core.chain;
namespace pcb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PcbParser parser = new PcbParser();
                StraightCbChain.setRowCbLimit(8);
                File.WriteAllLines("test.txt", parser.getOOC(input.Text, new StraightCbChain(new int[] {0,2,0})));
                if (parser.checkForCondDir().Length > 0)
                    MessageBox.Show(parser.checkForCondDir());
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
