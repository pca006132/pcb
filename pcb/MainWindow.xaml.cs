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

using pcb.core.autocomplete;
namespace pcb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tree tree;
        List<string> displayData = new List<string>();
        List<string> completionData = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                tree = InitAutocomplete.init();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            updateList();
            input.Focus();
        }

        private void updateList()
        {
            try
            {
                List<string>[] data = tree.autocomplete(input.Text);
                string item = "";
                if (listBox.SelectedIndex > -1)
                    item = displayData[listBox.SelectedIndex];
                displayData = data[0];
                completionData = data[1];
                listBox.Items.Clear();
                displayData.ForEach(s => listBox.Items.Add(s));
                if (displayData.Contains(item))
                    listBox.SelectedIndex = displayData.IndexOf(item);
                else
                    listBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateList();
        }

        private void input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (listBox.Items.Count > 1)
                        if (listBox.SelectedIndex < listBox.Items.Count - 1)
                            listBox.SelectedIndex++;
                        else
                            listBox.SelectedIndex = 0;
                    break;
                case Key.Up:
                    if (listBox.Items.Count > 1)
                        if (listBox.SelectedIndex > 0)
                            listBox.SelectedIndex--;
                        else
                            listBox.SelectedIndex = listBox.Items.Count - 1;
                    break;
                case Key.Tab:
                case Key.Enter:
                    if (completionData != null && completionData.Count > 0)
                        input.AppendText(completionData[listBox.SelectedIndex]);
                    input.SelectionStart = input.Text.Length;
                    e.Handled = true;
                    break;
            }
            listBox.ScrollIntoView(listBox.SelectedItem);
            input.Focus();
        }
    }
}
