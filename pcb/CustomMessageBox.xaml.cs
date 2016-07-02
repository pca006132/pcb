using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox
    {
        public static State ShowMessage(string message, string title, bool showYesNo = true)
        {
            CustomMessageBox msg;
            msg = new CustomMessageBox(message, title, showYesNo);
            
            State state = msg.state;
            msg.shouldClose = true;
            msg.Close();
            return state;
        }

        public CustomMessageBox(string message, string title, bool showYesNo)
        {            
            InitializeComponent();
            if (!showYesNo)
            {
                btnNo.Visibility = Visibility.Hidden;
                btnYes.Visibility = Visibility.Hidden;
            }
            this.message.Text = message;
            Title = title;
            if (this.message.ActualHeight > 100)
                Height = 250;           
            ShowDialog();
        }

        public enum State
        {
            close, no, yes
        }
        public State state = State.close;
        public bool shouldClose = false;
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            state = State.no;
            Hide();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            state = State.yes;
            Hide();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!shouldClose)
            {
                Hide();
                e.Cancel = true;
            }
        }

    }
}
