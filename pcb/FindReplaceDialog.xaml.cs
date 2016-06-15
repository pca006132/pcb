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
using System.Text.RegularExpressions;
using MahApps.Metro.Controls.Dialogs;

namespace pcb
{
    /// <summary>
    /// Interaction logic for FindReplaceDialog.xaml
    /// </summary>
    public partial class FindReplaceDialog
    {
        MainWindow parent;
        public FindReplaceDialog(MainWindow window)
        {
            InitializeComponent();
            parent = window;
        }
        private void showMessage(string text, string title)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = Properties.UIresources.close
            };

            this.ShowMessageAsync(title, text, MessageDialogStyle.Affirmative, mySettings);
        }
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void findNext()
        {
            var Editor = parent.Editor;
            try
            {
                int selectionStart = Editor.SelectionStart;
                selectionStart += Editor.SelectionLength;
                if (!(bool)UseRegex.IsChecked)
                {
                    if (!Editor.Text.Substring(selectionStart).Contains(findTB.Text))
                    {
                        System.Media.SystemSounds.Beep.Play();
                        parent.notificationTB.Text = Properties.UIresources.noMatch;
                        return;
                    }
                    int index = Editor.Text.Substring(selectionStart).IndexOf(findTB.Text) + selectionStart;
                    if (index > -1)
                    {
                        Editor.Select(index, findTB.Text.Length);
                        Editor.TextArea.Caret.BringCaretToView();
                    }
                }
                else
                {
                    if (Regex.IsMatch(Editor.Text.Substring(selectionStart), findTB.Text))
                    {
                        var match = Regex.Match(Editor.Text.Substring(selectionStart), findTB.Text, (((bool)MultiLine.IsChecked) ? RegexOptions.Multiline : RegexOptions.None));
                        Editor.Select(match.Index + selectionStart, match.Length);
                        Editor.TextArea.Caret.BringCaretToView();
                    }
                    else
                    {
                        System.Media.SystemSounds.Beep.Play();
                        parent.notificationTB.Text = Properties.UIresources.noMatch;
                    }
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.ToString(), Properties.Resources.error);
            }
        }
        private void replaceAll()
        {
            var Editor = parent.Editor;
            try
            {
                if (!(bool)UseRegex.IsChecked)
                {
                    if (Editor.SelectionLength == 0)
                    {
                        string text = Editor.Text.Replace(findTB.Text, replaceTB.Text);
                        Editor.Document.Replace(0, Editor.Text.Length, text);
                    }
                    else
                    {
                        string text = Editor.SelectedText.Replace(findTB.Text, replaceTB.Text);
                        Editor.Document.Replace(Editor.SelectionStart, Editor.SelectionLength, text);
                    }
                }
                else
                {
                    if (Editor.SelectionLength == 0)
                    {
                        string text = Regex.Replace(Editor.Text, findTB.Text, Regex.Unescape(replaceTB.Text), (((bool)MultiLine.IsChecked) ? RegexOptions.Multiline : RegexOptions.None));
                        Editor.Document.Replace(0, Editor.Text.Length, text);
                    }
                    else
                    {
                        string text = Regex.Replace(Editor.SelectedText, findTB.Text, Regex.Unescape(replaceTB.Text), (((bool)MultiLine.IsChecked) ? RegexOptions.Multiline : RegexOptions.None));
                        Editor.Document.Replace(Editor.SelectionStart, Editor.SelectionLength, text);
                    }
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.ToString(), Properties.Resources.error);
            }
        }

        private void btnFindNext_Click(object sender, RoutedEventArgs e)
        {
            findNext();
            parent.Editor.Focus();
        }
        private void btnReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            replaceAll();
            parent.Editor.Focus();
        }
    }    
}
