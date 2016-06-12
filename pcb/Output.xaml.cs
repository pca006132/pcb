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
                Title = "Output 命令过长，OOC:(1/" + text.Length.ToString() + ")";
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
            Clipboard.SetText(temp.ToString());
            this.Close();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox.Text);
            count++;
            if (count > texts.Length)
            {
                this.Close();
                return;
            }
            textBox.Text = texts[count - 1];
            Title = "Output 命令过长，OOC:(" + (count).ToString() + "/" + texts.Length.ToString() + ")";
        }
    }
}
