using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace pcb.core.autocomplete
{
    public class Value : IEquatable<Value>
    {
        //for references
        private static Dictionary<string, List<string>> references = new Dictionary<string, List<string>>();
        public static void addRef(string key, List<string> reference)
        {
            references.Add(key, reference);
        }
        public static List<string> getRef(string key)
        {
            if (references.ContainsKey(key))
                return references[key];
            return null;
        }
        public static bool lazyMatch = true;

        public static TreeNode sounds = null;
        public static void setSoundJson(string jsonStr)
        {
            string jsonString = jsonStr;
            try
            {
                JObject json = JObject.Parse(jsonString);
                List<string> keys = new List<string>();
                foreach (var pair in json)
                {
                    keys.Add(pair.Key);
                }
                sounds = TreeNode.generateTree(keys);
            }
            catch
            {
                throw new System.IO.InvalidDataException("Wrong JSON!");
            }
        }

        //something like a.b.c.d ...
        public static TreeNode attributes = new TreeNode("");
        public static void addAttributes(string name, string input)
        {
            if (!attributes.contains(name))
                attributes.Add(new TreeNode(name));
            foreach (string rawLine in input.Split('\n'))
            {
                string line = rawLine.Trim();
                string[] keys = line.Split('.');
                TreeNode temp = attributes.GetChild(name);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].StartsWith("#") && i == keys.Length - 1)
                    {
                        foreach (string str in references[keys[i].Substring(1)])
                        {
                            if (!temp.contains(str))
                                temp.Add(new TreeNode(str));
                        }
                    }
                    else
                    {
                        if (!temp.contains(keys[i]))
                            temp.Add(new TreeNode(keys[i]));
                        temp = temp.GetChild(keys[i]);
                    }
                }
            }
        }

        //for data in functions
        //can be used concurrently
        public static BlockingCollection<string> names = new BlockingCollection<string>();
        public static BlockingCollection<string> teams = new BlockingCollection<string>();
        public static BlockingCollection<string> tags = new BlockingCollection<string>();
        public static BlockingCollection<string> scbObj = new BlockingCollection<string>();
        public static BlockingCollection<string> triggerObj = new BlockingCollection<string>();

        public enum Type{
            reference, //values[0] = value
            regex,  //values[0] = pattern
            options, //values = options
            function //values[0] = function name, values[1+] = parameters
        }
        public Type type;
        public List<string> values = new List<string>();
        public string prefix = "";

        public bool Equals(Value value)
        {
            if (type == value.type && prefix == value.prefix && values.Count == value.values.Count)
            {
                foreach (string str in values)
                {
                    if (!value.values.Contains(str))
                        return false;
                }
                return true;
            }
            return false;
        }

        public Value(string para)
        {
            if (para.StartsWith("["))
            {
                int index = para.IndexOf("]");
                prefix = para.Substring(1, index - 1);
                para = para.Substring(index + 1);
            }
            int commentIndex = para.IndexOf('\'');
            para = para.Substring(0, commentIndex);
            switch (para[0])
            {
                case '#':
                    type = Type.reference;
                    if (!references.ContainsKey(para.Substring(1)))
                        throw new AutocompleteParseException("unknown reference:" + para.Substring(1));
                    values.Add(para.Substring(1));
                    break;
                case '<':
                    type = Type.regex;
                    int closeBracketIndex = para.LastIndexOf('>');
                    string pattern = para.Substring(1, closeBracketIndex - 1);
                    switch (pattern)
                    {
                        case "number":
                            pattern = @"^-?\d+(\.\d+)?$";
                            break;
                        case "coor":
                            pattern = @"^~?(-?\d+(\.\d+)?)?$";
                            break;
                    }
                    values.Add(pattern);
                    break;
                case '{':
                    type = Type.options;
                    foreach (string str in para.Substring(1, Math.Min(para.LastIndexOf('}'), para.Length - 2)).Split('|'))
                    {
                        values.Add(str);
                    }
                    break;
                case '$':
                    type = Type.function;
                    if (para.Contains("("))
                    {
                        int index = para.IndexOf('(');
                        values.Add(para.Substring(1, index - 1));
                        foreach (string str in para.Substring(index + 1, para.Length - index - 2).Split(','))
                        {
                            values.Add(str);
                        }
                    }
                    else
                    {
                        values.Add(para.Substring(1));
                    }
                    break;
            }
        }
        public bool isMatch(string segment)
        {
            if (segment.StartsWith(prefix))
                segment = segment.Substring(prefix.Length - 1);
            switch (type)
            {
                case Type.function:
                    return true;
                case Type.reference:
                    if (lazyMatch)
                        return true;
                    else
                    {
                        return (references[values[0]].Contains(segment));                            
                    }
                case Type.regex:
                    return Regex.IsMatch(segment, values[0]);
                case Type.options:
                    return values.Contains(segment);
                default:
                    return false;           
            }
        }
        public List<string> getValues(string input)
        {
            List<string> result = new List<string>();
            string beginMatch = "";
            switch (type)
            {
                case Type.reference:
                    result = references[values[0]];
                    break;
                case Type.options:
                    result = values;
                    break;
                case Type.function:
                    switch (values[0])
                    {
                        case "scbObj":
                            result = scbObj.ToList();
                            break;
                        case "trigger":
                            result = triggerObj.ToList();
                            break;
                        case "team":
                            result = teams.ToList();
                            break;
                        case "tag":
                            result = tags.ToList();
                            break;
                        case "sound":
                            if (sounds != null)
                                result = sounds.Autocomplete(input);
                            beginMatch = input.Split('.').Last();
                            break;
                        case "dot":
                            if (attributes != null)
                                foreach (string value in values.Skip(1))
                                {
                                    result.AddRange(attributes.Autocomplete(value + "." + input));
                                }
                            beginMatch = input.Split('.').Last();
                            break;
                    }
                    break;
            }
            return result.Distinct().Where(s => s.StartsWith(beginMatch)).ToList();
        }
    }
}
