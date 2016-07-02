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
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ICSharpCode.AvalonEdit;
using System.Threading;
using MahApps.Metro.Controls.Dialogs;
using System.IO;

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
            scope.SetVariable("math", new math());
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
            string text = "import clr\nclr.AddReference('System.Xml')\n" + editor.Text;
            if (File.Exists("ref/py.py"))
            {
                text = File.ReadAllText("ref/py.py", Encoding.UTF8) + "\r\n" + text;
            }
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
                    Dispatcher.BeginInvoke((Action)(() => showMessage(ex.Message, "error!")));
                    try
                    {
                        if (!File.Exists("documents/log/log.txt"))
                            File.Create("documents/log/log.txt");
                        var writer = File.AppendText("documents/log/log.txt");
                        writer.WriteLine(ex.Message);
                        writer.WriteLine(ex.StackTrace);
                    }
                    catch { }
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

    public class editorMethod
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
            main_editor.Dispatcher.BeginInvoke((Action)(() => main_editor.Document.Insert(index, text)));
        }
        public void replaceText(int index, int length, string text)
        {
            main_editor.Dispatcher.BeginInvoke((Action)(() => main_editor.Document.Replace(index, length, text)));
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
        public string getSelectedText()
        {
            var text = "";
            main_editor.Dispatcher.BeginInvoke((Action)(() => text = main_editor.SelectedText));
            return text;
        }
        public void replaceSelectedText(string text)
        {
            main_editor.Dispatcher.BeginInvoke((Action)(() => main_editor.Document.Replace(main_editor.SelectionStart, main_editor.SelectionLength, text)));
        }
        public int getLineOffset(int lineNum)
        {
            int offset = -1;
            main_editor.Dispatcher.BeginInvoke((Action)(() => offset = main_editor.Document.GetLineByNumber(lineNum).Offset));
            return offset;
        }
        public int getLineLength(int lineNum)
        {
            int length = 0;
            main_editor.Dispatcher.BeginInvoke((Action)(() => length = main_editor.Document.GetLineByNumber(lineNum).Length));
            return length;
        }
        public int getLineNum(int offset)
        {
            int lineNum = -1;
            main_editor.Dispatcher.BeginInvoke((Action)(() => lineNum = main_editor.Document.GetLineByOffset(offset).LineNumber));
            return lineNum;
        }
        public void showMessage(string message, string title)
        {
            main_editor.Dispatcher.BeginInvoke((Action)(() => CustomMessageBox.ShowMessage(message, title, false)));
        }
    }
    public class Util
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
    public class math
    {
        Random rnd = new Random();

        public static double pi = Math.PI;
        public static int round(double num)
        {
            return (int)Math.Round(num);
        }
        public static double sin(double rad)
        {
            return Math.Sin(rad);
        }
        public static double cos(double rad)
        {
            return Math.Cos(rad);
        }
        public static double tan(double rad)
        {
            return Math.Tan(rad);
        }
        public static double radToDeg(double rad)
        {
            return rad / pi * 180;
        }
        public static double degToRad(double deg)
        {
            return deg * pi / 180;
        }
        public static double asin(double num)
        {
            return Math.Asin(num);
        }
        public static double acos(double num)
        {
            return Math.Acos(num);
        }
        public static double atan(double num)
        {
            return Math.Atan(num);
        }
        public static double atan2(double y, double x)
        {
            return Math.Atan2(y, x);            
        }
        public static double sinh(double num)
        {
            return Math.Sinh(num);
        }
        public static double cosh(double num)
        {
            return Math.Cosh(num);
        }
        public static double tanh(double num)
        {
            return Math.Tanh(num);
        }
        public static double exp(double num)
        {
            return Math.Exp(num);
        }
        public static double log(double num, double logBase)
        {
            return Math.Log(num, logBase);
        }
        public static double log10(double num)
        {
            return Math.Log10(num);            
        }
        public static double pow(double num, double power)
        {
            return Math.Pow(num, power);
        }
        public int randInt(int max)
        {
            return rnd.Next(max);
        }
        public double rand()
        {
            return rnd.NextDouble();
        }
    }
}
