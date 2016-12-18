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
    /// Interaction logic for Output.xaml
    /// </summary>
    public partial class Output
    {
        string[] texts;
        int count = 1;
        public Output(string[] text)
        {
            InitializeComponent();
            textBox.Text = text[0];
            texts = text;
            if (text.Length > 1)
                Title = String.Format(Properties.UIresources.ooc2Long,1,text.Length);
            Show();
        }
        private void copyAll(object sender, RoutedEventArgs e)
        {
            StringBuilder temp = new StringBuilder();
            foreach (string text in texts)
            {
                temp.Append(text);
                temp.Append("\n\n");
            }

            Clipboard.SetDataObject(temp.ToString());
            this.Close();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            string text = textBox.Text;
            Clipboard.SetDataObject(text);
            count++;
            if (count > texts.Length)
            {
                this.Close();
                return;
            }
            textBox.Text = texts[count - 1];
            Title = String.Format(Properties.UIresources.ooc2Long, count, texts.Length);
        }
    }
}
