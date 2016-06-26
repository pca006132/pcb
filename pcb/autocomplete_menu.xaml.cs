using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
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
    /// Interaction logic for autocomplete_menu.xaml
    /// </summary>
    public partial class autocomplete_menu
    {

        public TextEditor editor;
        protected int line;
        protected int column;
        protected MainWindow window;
        protected double y;
        public bool shown = false;

        public autocomplete_menu(TextEditor _editor, MainWindow _window)
        {
            InitializeComponent();
            editor = _editor;
            window = _window;
            window.LocationChanged += new EventHandler(Window_LocationChanged);            
            editor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

            update_position();
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            update_position();
        }
        
        public void update_position()
        {
            Point p = editor.TextArea.TextView.GetVisualPosition(editor.TextArea.Caret.Position, VisualYPosition.LineBottom) - editor.TextArea.TextView.ScrollOffset;
            double Left = this.Left;
            double Top = this.Top;
            Top = window.Top + p.Y + 65;
            y = Top;
            if (p.X  + listbox.ActualWidth + 55 > window.ActualWidth)
                Left = Math.Abs(window.Left + window.ActualWidth - 180);
            else
                Left = window.Left + p.X + 55;
            if (window.WindowState == WindowState.Maximized)
            {
                Left -= window.Left;
                Top -= window.Top;
            }
            this.Left = Left;
            this.Top = Top;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (shown)
                update_position();
        }
        private void listbox_GotFocus(object sender, RoutedEventArgs e)
        {
            editor.Focus();
            Hide();
            Show();            
        }
    }
}
