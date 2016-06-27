using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit;
using System.Windows.Input;
using System.Windows;

namespace pcb
{
    class Snippet_menu : autocomplete_menu
    {
        public List<Snippet> snippets = new List<Snippet>();
        public List<string> names;
        int startOffset = -1;
        List<Snippet> storage;
        string input = "";
        public Snippet_menu(TextEditor _editor, MainWindow _window, List<Snippet> snippets, List<string> snippets_names) : base(_editor, _window)
        {
            storage = snippets;
            names = snippets_names;
            
            editor.TextChanged += Editor_TextChanged;
            editor.PreviewKeyDown += editor_keyDown;
            listbox.MouseDoubleClick += listbox_MouseDoubleClick;
            editor.TextArea.Caret.PositionChanged += new EventHandler(caretChanged);
        }


        public void show()
        {
            Hide();
            line = editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber;
            column = editor.CaretOffset - editor.Document.GetLineByOffset(editor.CaretOffset).Offset;
            startOffset = editor.SelectionStart;
            update_position();
            listbox.Items.Clear();
            snippets.Clear();
            for (int i = 0; i < names.Count; i++)
            {                
                listbox.Items.Add(names[i]);
                snippets.Add(storage[i]);
                
            }
            Show();
            shown = true;
            listbox.SelectedIndex = 0;
        }        
        void Editor_TextChanged(object sender, EventArgs e)
        {
            if (shown)
            {
                if (startOffset >= editor.SelectionStart)
                {
                    listbox.Items.Clear();
                    snippets.Clear();
                    Hide();
                    shown = false;
                    return;
                }
                input = editor.Document.GetText(startOffset, editor.SelectionStart - startOffset);
                listbox.Items.Clear();
                snippets.Clear();
                line = editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber;
                column = editor.CaretOffset - editor.Document.GetLineByOffset(editor.CaretOffset).Offset;
                for (int i = 0; i < storage.Count; i++)
                {
                    if (names[i].StartsWith(input))
                    {
                        listbox.Items.Add(names[i]);
                        snippets.Add(storage[i]);
                    }
                }
                listbox.SelectedIndex = 0;
                if (listbox.Items.Count == 0)
                {
                    Hide();
                    shown = false;
                }
            }
        }
        void editor_keyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (shown)
                {
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
                        Hide();
                        shown = false;
                        e.Handled = true;
                        if (input.Length > 0)                        
                            editor.Document.Remove(editor.SelectionStart - input.Length, input.Length);                        
                        input = "";
                        snippets[listbox.SelectedIndex].Insert(editor.TextArea);
                    }
                    if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                    {                        
                        shown = false;
                        Hide();
                        if (input.Length > 0)
                            editor.Document.Remove(editor.SelectionStart - input.Length, input.Length);
                        input = "";
                        snippets[listbox.SelectedIndex].Insert(editor.TextArea);
                    }
                }
            }
            catch (Exception ex)
            {
                window.log(ex);
            }
        }
        void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (shown)
                {
                    shown = false;
                    Hide();
                    input = "";
                    snippets[listbox.SelectedIndex].Insert(editor.TextArea);
                }
            } catch { }
        }
        void caretChanged(object sender, EventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                if (line != editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber)
                {
                    input = "";
                    listbox.Items.Clear();
                    Hide();
                }
                else if (Math.Abs(column - (editor.CaretOffset - editor.Document.GetLineByOffset
                    (editor.CaretOffset).Offset)) > 2)
                {
                    input = "";
                    listbox.Items.Clear();
                    Hide();
                }
            }
        }
    }
}
