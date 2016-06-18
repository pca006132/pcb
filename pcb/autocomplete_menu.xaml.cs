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
        public List<string> display = new List<string>();
        public List<int> completion = new List<int>();
        public TextEditor editor;
        int line;
        int column;
        MainWindow window;
        double y;
        public bool shown = false;

        public void updateitem(List<string> display, List<int> completion)
        {
            line = editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber;
            column = editor.CaretOffset - editor.Document.GetLineByOffset(editor.CaretOffset).Offset;
            string selected = "";
            if (listbox.SelectedIndex > -1 && listbox.SelectedIndex < this.display.Count)
                selected = this.display[listbox.SelectedIndex];
            listbox.Items.Clear();
            this.display = display;
            this.completion = completion;
            foreach (string str in display)
            {
                listbox.Items.Add(str);
            }
            if (!listbox.HasItems)
            {
                Hide();
                shown = false;
                return;
            }
            if (display.Contains(selected))
                listbox.SelectedIndex = display.IndexOf(selected);            
            for (int i = 0; i < display.Count; i++)
            {
                if (completion[i] == display[i].Length)
                {
                    Hide();
                    shown = false;
                    return;
                }
            }
            update_position();
            Show();
            shown = true;
            listbox.SelectedIndex = 0;
        }
        public autocomplete_menu(TextEditor _editor, MainWindow _window)
        {
            InitializeComponent();
            editor = _editor;
            window = _window;
            window.LocationChanged += new EventHandler(Window_LocationChanged);
            editor.PreviewKeyDown += new KeyEventHandler(editor_keyDown);
            editor.TextArea.Caret.PositionChanged += new EventHandler(caretChanged);
            editor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

            update_position();
        }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            update_position();
        }

        private void caretChanged(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                if (line != editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber)
                {
                    display.Clear();
                    completion.Clear();
                    listbox.Items.Clear();
                    Hide();
                    return;
                }
                if (Math.Abs(column - (editor.CaretOffset - editor.Document.GetLineByOffset
                    (editor.CaretOffset).Offset)) > 2)
                {
                    display.Clear();
                    completion.Clear();
                    listbox.Items.Clear();
                    this.Hide();
                    return;
                };
            }
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
        private void editor_keyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (shown && listbox.HasItems)
                {
                    if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                    {                        
                        if (Keyboard.IsKeyDown(Key.A))
                        {
                            DocumentLine line = editor.Document.GetLineByOffset(editor.CaretOffset);
                            string before = editor.Document.GetText(line.Offset, editor.CaretOffset - line.Offset);
                            string after = editor.Document.GetText(editor.CaretOffset, line.EndOffset - editor.CaretOffset);
                            StringBuilder temp = new StringBuilder();                            
                            for (int i = 0; i < display.Count; i++)
                            {
                                temp.Append(before);
                                temp.Remove(temp.Length - completion[i], completion[i]);
                                temp.Append(display[i]);
                                temp.AppendLine(after);
                            }                            
                            editor.Document.Replace(line, temp.ToString());
                        }
                    }
                    if (e.Key == Key.Up)
                    {
                        if (listbox.SelectedIndex <= 0)
                            listbox.SelectedIndex = listbox.Items.Count - 1;
                        else
                            listbox.SelectedIndex--;
                        listbox.ScrollIntoView(listbox.SelectedItem);
                        e.Handled = true;
                    }
                    if (e.Key == Key.Down)
                    {
                        if (listbox.SelectedIndex >= listbox.Items.Count - 1)
                            listbox.SelectedIndex = 0;
                        else
                            listbox.SelectedIndex++;
                        listbox.ScrollIntoView(listbox.SelectedItem);
                        e.Handled = true;
                    }
                    if (e.Key == Key.Enter || e.Key == Key.Tab)
                    {
                        editor.Document.Replace(editor.SelectionStart - completion[listbox.SelectedIndex], completion[listbox.SelectedIndex], display[listbox.SelectedIndex]);
                        display.Clear();
                        completion.Clear();
                        listbox.Items.Clear();
                        Hide();
                        e.Handled = true;
                    }
                    if (e.Key == Key.Space || e.Key == Key.Decimal ||  e.Key == Key.OemPeriod)
                    {
                        editor.Document.Replace(editor.SelectionStart - completion[listbox.SelectedIndex], completion[listbox.SelectedIndex], display[listbox.SelectedIndex]);
                        display.Clear();
                        completion.Clear();
                        listbox.Items.Clear();
                        Hide();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                window.log(ex);
            }
        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (shown)
                update_position();
        }
        private void listbox_GotFocus(object sender, RoutedEventArgs e)
        {
            editor.Focus();
        }
        private void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editor.Document.Replace(editor.SelectionStart - completion[listbox.SelectedIndex], completion[listbox.SelectedIndex], display[listbox.SelectedIndex]);
            display.Clear();
            completion.Clear();
            listbox.Items.Clear();
            Hide();
        }
    }
}
