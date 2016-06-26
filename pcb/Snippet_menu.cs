using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit;
using System.Windows.Input;

namespace pcb
{
    class Snippet_menu : autocomplete_menu
    {
        public List<Snippet> snippets;
        public Snippet_menu(TextEditor _editor, MainWindow _window, List<Snippet> snippets, List<string> snippets_names) : base(_editor, _window)
        {
            this.snippets = snippets;
            foreach (string item in snippets_names)
            {
                listbox.Items.Add(item);
            }
            editor.PreviewKeyDown += new KeyEventHandler(editor_keyDown);
            listbox.MouseDoubleClick += listbox_MouseDoubleClick;
            editor.TextArea.Caret.PositionChanged += new EventHandler(caretChanged);
        }
        public void show()
        {
            Hide();
            update_position();
            Show();
            shown = true;
            listbox.SelectedIndex = 0;
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
                        snippets[listbox.SelectedIndex].Insert(editor.TextArea);
                        Hide();
                        shown = false;
                        e.Handled = true;
                    }
                    if (e.Key == Key.Space || e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                    {
                        snippets[listbox.SelectedIndex].Insert(editor.TextArea);
                        shown = false;
                        Hide();
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
                    snippets[listbox.SelectedIndex].Insert(editor.TextArea);
                    shown = false;
                    Hide();
                }
            } catch { }
        }
        void caretChanged(object sender, EventArgs e)
        {
            if (shown)
            {
                Hide();
                shown = false;
            }
        }
    }
}
