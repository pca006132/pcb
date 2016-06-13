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
using MahApps.Metro.Controls;
using pcb.core.autocomplete;

namespace pcb
{
    /// <summary>
    /// Interaction logic for add_ac_list.xaml
    /// </summary>
    public partial class add_ac_list
    {
        int current_state = 0;
        public add_ac_list()
        {
            InitializeComponent();
            list_type.SelectedIndex = 0;
            update();
        }

        private void add()
        {
            switch (current_state)
            {
                case 0:
                    Value.scbObj.Add(textBox.Text);
                    break;
                case 1:
                    Value.tags.Add(textBox.Text);
                    break;
                case 2:
                    Value.teams.Add(textBox.Text);
                    break;
                case 3:
                    Value.names.Add(textBox.Text);
                    break;
            }
            textBox.Text = "";
            update();
        }

        private void update()
        {
            list_content.Items.Clear();
            var list = Value.scbObj;
            switch (current_state)
            {
                case 0:
                    break;
                case 1:
                    list = Value.tags;
                    break;
                case 2:
                    list = Value.teams;
                    break;
                case 3:
                    list = Value.names;
                    break;
            }
            foreach (var item in list)
            {
                list_content.Items.Add(item);
            }
        }

        private void list_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            current_state = list_type.SelectedIndex;
            update();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                add();
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            add();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            if (list_content.SelectedIndex > -1)
            {
                string item;
                switch (current_state)
                {
                    case 0:
                        item = Value.scbObj.ElementAt(list_content.SelectedIndex);
                        Value.scbObj.TryTake(out item);
                        break;
                    case 1:
                        item = Value.tags.ElementAt(list_content.SelectedIndex);
                        Value.tags.TryTake(out item);
                        break;
                    case 2:
                        item = Value.teams.ElementAt(list_content.SelectedIndex);
                        Value.teams.TryTake(out item);
                        break;
                    case 3:
                        item = Value.names.ElementAt(list_content.SelectedIndex);
                        Value.names.TryTake(out item);
                        break;
                }
            }
            update();
        }
    }
}
