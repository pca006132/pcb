using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb
{
    class PrintNbt
    {
        private const string INDENT_STRING = "    ";
        public static string FormatNbt(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            ++indent;
                            for (int count = 0; count < indent; count++)
                            {
                                sb.Append(INDENT_STRING);
                            }
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            --indent;
                            for (int count = 0; count < indent; count++)
                            {
                                sb.Append(INDENT_STRING);
                            }
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            for (int count = 0; count < indent; count++)
                            {
                                sb.Append(INDENT_STRING);
                            }
                        }
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
        public static string NbtToCommand(string str)
        {
            var lines = str.Split('\n');
            var sb = new StringBuilder();            
            Array.ForEach(lines, item => sb.Append(item.Trim()));
            return sb.ToString();
        }
    }
}
