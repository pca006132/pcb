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

using pcb.core.expression;
using MahApps.Metro.Controls.Dialogs;

namespace pcb
{
    /// <summary>
    /// Interaction logic for expression.xaml
    /// </summary>
    public partial class Expression
    {
        public Expression()
        {
            InitializeComponent();
        }

        private void gen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Node node = (new NodeBuilder(input.Text)).buildNode();
                output.Text = string.Join("\n", node.getCommands());
            } catch (Exception ex)
            {
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = Properties.UIresources.close
                };

                this.ShowMessageAsync(Properties.Resources.error, ex.Message + "\n" + ex.StackTrace,
                    MessageDialogStyle.Affirmative, mySettings);
            }
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = Properties.UIresources.close
            };

            this.ShowMessageAsync(Properties.UIresources.help, Properties.Resources.math_help.Replace("\\n", "\n"),
                MessageDialogStyle.Affirmative, mySettings);
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(output.Text);
        }
    }
}
