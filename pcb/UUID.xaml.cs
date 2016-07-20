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
using pcb.core.util;
using System.Text.RegularExpressions;

namespace pcb
{
    /// <summary>
    /// Interaction logic for UUID.xaml
    /// </summary>
    public partial class UUID
    {
        public UUID()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long[] pair = CommandUtil.randomUUIDPair();
            id_most.Text = pair[0].ToString();
            id_least.Text = pair[1].ToString();

            string id = CommandUtil.UUIDPairToString(pair[0], pair[1]);
            uuid.Text = id;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                long most = long.Parse(id_most.Text);
                long least = long.Parse(id_least.Text);
                uuid.Text = CommandUtil.UUIDPairToString(most, least);
            }
            catch (FormatException)
            {
                CustomMessageBox.ShowMessage("most/least is not long", "error", false);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string id = uuid.Text.Replace("-", "");
            if (!Regex.IsMatch(id, @"^[0-9a-fA-F]{32}$"))
            {
                CustomMessageBox.ShowMessage("Invalid UUID", "error", false);
                return;
            }
            long[] pair = CommandUtil.UUIDGetLeastMostFromString(id);
            id_most.Text = pair[0].ToString();
            id_least.Text = pair[1].ToString();
        }
    }
}
