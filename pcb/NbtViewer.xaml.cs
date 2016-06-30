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

namespace pcb
{
    /// <summary>
    /// Interaction logic for NbtViewer.xaml
    /// </summary>
    public partial class NbtViewer
    {
        public static string viewNBT(string str)
        {
            NbtViewer viewer = new NbtViewer(str);
            viewer.ShowDialog();
            return PrintNbt.NbtToCommand(viewer.text);
        }
        public string text;
        public NbtViewer(string str)
        {
            InitializeComponent();
            editor.Text = PrintNbt.FormatNbt(str);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            text = editor.Text;
        }
    }
}
