using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace pcb.core.autocomplete
{
    public class Value
    {
        //for references
        private static Dictionary<string, List<string>> references = new Dictionary<string, List<string>>();
        public static void addRef(string key, List<string> reference)
        {
            if (!references.ContainsKey(key))
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
        public static void addAttributes(string name, List<string> input)
        {
            if (!attributes.contains(name))
                attributes.Add(new TreeNode(name));
            foreach (string element in input)
            {
                string line = element.Trim();
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

        private static readonly string[] functionNames = {"scbObj" ,"trigger","team","tag","sound","dot","selector"};

        public enum Type{
            reference, //values[0] = value key
            regex,  //values[0] = pattern
            options, //values = options
            function, //values[0] = function name, values[1+] = parameters
            normal //values[0] = value
        }
        public Type type;
        public List<string> values = new List<string>();
        public string prefix = "";
        public string rawValue = "";

        public Value(string para)
        {
            rawValue = para;
            if (para.StartsWith("["))
            {
                int index = para.IndexOf("]");
                prefix = para.Substring(1, index - 1);
                para = para.Substring(index + 1);
            }
            int commentIndex = para.IndexOf('\'');
            if (commentIndex < 0)
                commentIndex = para.Length;            
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
                    if (closeBracketIndex < 0)
                        throw new AutocompleteParseException("missing ending bracket > ");
                    string pattern = para.Substring(1, closeBracketIndex - 1);
                    switch (pattern)
                    {
                        case "number":
                            pattern = @"^-?\d+(\.\d+)?$";
                            break;
                        case "coor":
                            pattern = @"^(~?(-?\d+(\.\d+)?)|~)$";
                            break;
                        case "int":
                            pattern = @"^-?\d+$";
                            break;
                    }
                    values.Add(pattern);
                    break;
                case '{':
                    type = Type.options;
                    if (!para.Contains("}"))
                        throw new AutocompleteParseException("missing ending bracket } ");
                    foreach (string str in para.Substring(1, Math.Min(para.LastIndexOf('}'), para.Length - 2)).Split('|'))
                    {
                        values.Add(str);
                    }
                    break;
                case '$':
                    type = Type.function;
                    if (para.Contains("("))
                    {
                        if (!para.Contains(")"))
                            throw new AutocompleteParseException("missing ending bracket ) ");
                        int index = para.IndexOf('(');
                        values.Add(para.Substring(1, index - 1));

                        if (!functionNames.Contains(values[0]))
                            throw new AutocompleteParseException("unknown function name");
                        foreach (string str in para.Substring(index + 1, para.Length - index - 2).Split(','))
                        {
                            values.Add(str);
                            if (values[0] == "dot")
                                if (!attributes.contains(str))
                                    throw new AutocompleteParseException("unknown dot attribute");
                        }
                    }
                    else
                    {
                        values.Add(para.Substring(1));
                    }
                    break;
                default:
                    type = Type.normal;
                    values.Add(para);
                    break;
            }
        }
        public bool isMatch(string segment)
        {
            if (prefix.Length > 0 && segment.StartsWith(prefix))
                segment = segment.Substring(prefix.Length);
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
                case Type.normal:
                    return values[0].Equals(segment);
                default:
                    return false;           
            }
        }

        public List<string>[] getValues(string input)
        {
            List<string> result = new List<string>();
            string beginMatch = input;
            if (prefix.Length > 0 && beginMatch.StartsWith(prefix))
                beginMatch = beginMatch.Substring(prefix.Length);
            switch (type)
            {
                case Type.reference:
                    result = references[values[0]];
                    break;
                case Type.options:
                case Type.normal:
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
                        case "selector":
                            if ((input.StartsWith("@a[") || input.StartsWith("@p[") || input.StartsWith("@e[") || input.StartsWith("@r[")) && input.LastIndexOf(',') >= input.LastIndexOf('='))
                            {
                                input = input.Substring(3);
                                result.Add("x");
                                result.Add("y");
                                result.Add("z");
                                result.Add("dx");
                                result.Add("dy");
                                result.Add("dz");
                                result.Add("r");
                                result.Add("rm");
                                result.Add("c");
                                result.Add("m");
                                result.Add("l");
                                result.Add("lm");
                                result.Add("team");
                                result.Add("name");
                                result.Add("rx");
                                result.Add("rxm");
                                result.Add("ry");
                                result.Add("rym");
                                result.Add("type");
                                result.Add("tag");
                                foreach (string objective in scbObj)
                                {
                                    result.Add("score_" + objective + "_min");
                                    result.Add("score_" + objective);
                                }
                                string[] elements = input.Split(',');
                                if (elements.Length > 1)
                                    foreach (string element in elements.Take(elements.Length - 1))
                                    {
                                        string name = element.Split('=')[0];
                                        if (result.Contains(name))
                                            result.Remove(name);
                                    }
                                beginMatch = input.Split(',').Last();
                            }
                            else if ((input.StartsWith("@a[") || input.StartsWith("@p[") || input.StartsWith("@e[") || input.StartsWith("@r[")) && input.LastIndexOf(',') < input.LastIndexOf('=') && input.LastIndexOf('=') > -1)
                            {
                                var match = Regex.Match(input, @"\W([a-zA-Z_]*)=!?(\w*)$");
                                string name = match.Groups[1].ToString();
                                if (name == "type")
                                {
                                    if (references.ContainsKey("entityID"))
                                        result = new List<string>(references["entityID"]);
                                    result.Add("Player");
                                }
                                else if (name == "team")
                                {
                                    result = teams.ToList();
                                }
                                else if (name == "tag")
                                {
                                    result = tags.ToList();
                                }
                                else if (name == "name")
                                {
                                    result = names.ToList();
                                }
                                beginMatch = input.Split('=', '!').Last();
                            }
                            break;
                    }
                    break;
            }
            result = result.Distinct().Where(s => s.StartsWith(beginMatch)).ToList();
            result.Sort();
            List<string> complete = new List<string>(result);
            for (int i = 0; i < complete.Count; i++)
            {
                complete[i] = complete[i].Substring(beginMatch.Length);
            }
            return new List<string>[] {result, complete};
        }
    }
}
