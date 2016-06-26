using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Windows.Input;

namespace pcb
{
    class autocomplete_menu_data : autocomplete_menu
    {
        public List<string> display = new List<string>();
        public List<int> completion = new List<int>();

        public autocomplete_menu_data(TextEditor _editor, MainWindow _window) : base(_editor, _window)
        {
            editor.PreviewKeyDown += new KeyEventHandler(editor_keyDown);
            listbox.MouseDoubleClick += listbox_MouseDoubleClick;
            editor.TextArea.Caret.PositionChanged += new EventHandler(caretChanged);
        }
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
                    if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                    {
                        editor.Document.Replace(editor.SelectionStart - completion[listbox.SelectedIndex], completion[listbox.SelectedIndex], display[listbox.SelectedIndex]);
                        display.Clear();
                        completion.Clear();
                        listbox.Items.Clear();
                        Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                window.log(ex);
            }
        }
        private void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            editor.Document.Replace(editor.SelectionStart - completion[listbox.SelectedIndex], completion[listbox.SelectedIndex], display[listbox.SelectedIndex]);
            display.Clear();
            completion.Clear();
            listbox.Items.Clear();
            Hide();
        }
        private void caretChanged(object sender, EventArgs e)
        {
            if (Visibility == Visibility.Visible)
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
    }
}
