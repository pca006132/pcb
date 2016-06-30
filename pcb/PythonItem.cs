using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Windows.Controls;
using System.Windows;

namespace pcb
{
    public class PythonItem : MenuItem
    {
        public static List<PythonItem> readAllPys(TextEditor _editor, MainWindow _window)
        {
            List<PythonItem> result = new List<PythonItem>();
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope;
            scope = engine.CreateScope();
            scope.SetVariable("editor", new editorMethod(_editor));
            scope.SetVariable("util", new Util());
            scope.SetVariable("math", new math());
            try
            {
                foreach (string fileName in Directory.GetFiles("ref/py"))
                {
                    string file = "import clr\nclr.AddReference('System.Xml')\n" + File.ReadAllText(fileName, Encoding.UTF8);
                    string name = fileName.Split('.')[0].Split('/', '\\').Last();
                    try
                    {
                        var compiled = engine.CreateScriptSourceFromString(file).Compile();
                        result.Add(new PythonItem(engine, scope, name, compiled));
                    }
                    catch { }
                }
            } catch { }
            return result;
        }
        ScriptEngine engine;
        ScriptScope scope;
        string name;
        CompiledCode code;
        public PythonItem(ScriptEngine engine, ScriptScope scope, string name, CompiledCode code)
        {
            this.engine = engine;
            this.scope = scope;
            this.name = name;
            this.code = code;

            Header = name;
            PreviewMouseDown += PythonItem_MouseDown;
        }
        private void PythonItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                code.Execute(scope);
            } catch (Exception ex)
            {
                CustomMessageBox.ShowMessage(ex.ToString(), Properties.Resources.error);
            }
        }
    }
}
