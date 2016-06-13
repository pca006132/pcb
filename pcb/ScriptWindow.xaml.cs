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
using pcb.core.autocomplete;
using pcb.core.util;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ICSharpCode.AvalonEdit;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;

namespace pcb
{
    /// <summary>
    /// Interaction logic for ScriptWindow.xaml
    /// </summary>
    public partial class ScriptWindow
    {
        ScriptEngine engine = Python.CreateEngine();
        ScriptScope scope;
        Thread pythonThread;
        public ScriptWindow(TextEditor editor)
        {
            InitializeComponent();
            scope = engine.CreateScope();
            scope.SetVariable("editor", new editorMethod(editor));
            scope.SetVariable("util", new Util());
            Show();
            
        }
        public void showMessage(string text, string title)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "关闭"
            };

            this.ShowMessageAsync(title, text, MessageDialogStyle.Affirmative, mySettings);
        }
        void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = editor.Text;
            pythonThread = new Thread(() =>
            {
                try
                {                    
                    ScriptSource source =
            engine.CreateScriptSourceFromString(text, SourceCodeKind.Statements);

                    source.Execute(scope);
                }
                catch (ThreadAbortException tae)
                {
                    if (tae.ExceptionState is KeyboardInterruptException)
                    {
                        Thread.ResetAbort();
                    }
                    else { throw; }
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke((Action)(()=>showMessage(ex.StackTrace, "error!")));                    
                }
            });
            pythonThread.Start();
        }
        void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (pythonThread != null)
                pythonThread.Abort(new KeyboardInterruptException(""));
        }
    }

    class editorMethod
    {
        TextEditor main_editor;
        public editorMethod(TextEditor editor)
        {
            main_editor = editor;
        }

        public void appendText(string text)
        {
            main_editor.Dispatcher.BeginInvoke((Action)(() => main_editor.Document.Insert(main_editor.Text.Length, text)));
        }
        public void insertText(int index, string text)
        {
            main_editor.Dispatcher.BeginInvoke((Action)(()=> main_editor.Document.Insert(index, text)));
        }
        public void replaceText(int index, int length, string text)
        {
            main_editor.Dispatcher.BeginInvoke((Action)(()=> main_editor.Document.Replace(index, length, text)));
        }
        public string getText()
        {
            string text = "";
            main_editor.Dispatcher.Invoke((Action)(() => text = main_editor.Text));
            return text;
        }
        public int getSelectionStart()
        {
            var index = -1;
            main_editor.Dispatcher.Invoke((Action)(() => index = main_editor.SelectionStart));
            return index;
        }
        public int getSelectionLength()
        {
            var length = -1;
            main_editor.Dispatcher.Invoke((Action)(() => length = main_editor.SelectionLength));
            return length;
        }       
    }
    class Util
    {
        public string[] getReference(string key)
        {
            return Value.getRef(key).ToArray();
        }
        public string[] getScbObjs()
        {
            return Value.scbObj.ToArray();
        }
        public string[] getTeams()
        {
            return Value.teams.ToArray();
        }
        public string[] getTags()
        {
            return Value.tags.ToArray();
        }
        public string[] getNames()
        {            
            return Value.names.ToArray();
        }
        public string escape(string text)
        {
            return CommandUtil.escape(text);
        }
        public string unescape(string text)
        {
            return CommandUtil.unescape(text);
        }
        public string uuidToString(long uuidLeast, long uuidMost)
        {
            return CommandUtil.UUIDPairToString(uuidMost, uuidLeast);
        }        
    }
}
