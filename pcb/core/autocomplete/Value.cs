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
        public static bool forceCompletePrefix = false;

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
            return new List<string>();
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
        public static List<string> names = new List<string>();
        public static List<string> teams = new List<string>();
        public static List<string> tags = new List<string>();
        public static List<string> scbObj = new List<string>();
        public static List<string> triggerObj = new List<string>();

        public static List<string> runtime_names = new List<string>();
        public static List<string> runtime_teams = new List<string>();
        public static List<string> runtime_tags = new List<string>();
        public static List<string> runtime_scbObj = new List<string>();
        public static List<string> runtime_triggerObj = new List<string>();

        private static readonly string[] functionNames = {"scbObj" ,"trigger","team","tag","sound","dot","selector","command","nbt"};

        public enum Type{
            reference, //values[0] = value key
            regex,  //values[0] = pattern
            options, //values = options
            function, //values[0] = function name, values[1+] = parameters
            normal //values[0] = value
        }
        public Type type;
        public List<string> values = new List<string>();
        public List<string> prefix = new List<string>();
        public string rawValue = "";

        public Value(string para)
        {
            rawValue = para;
            while (para.StartsWith("["))
            {
                int index = para.IndexOf("]");
                prefix.Add(para.Substring(1, index - 1));
                para = para.Substring(index + 1);
            }
            int commentIndex = para.IndexOf('\'');
            if (commentIndex < 0)
                commentIndex = para.Length;            
            para = para.Substring(0, commentIndex);
            
            if (para.Length == 0)
            {
                throw new AutocompleteParseException(Properties.Resources.emptyArgument);
            }

            switch (para[0])
            {
                case '#':
                    type = Type.reference;
                    if (!references.ContainsKey(para.Substring(1)))
                        throw new AutocompleteParseException(Properties.Resources.unknownReference + ": " + para.Substring(1));
                    values.Add(para.Substring(1));
                    break;
                case '<':
                    type = Type.regex;
                    int closeBracketIndex = para.LastIndexOf('>');
                    if (closeBracketIndex < 0)
                        throw new AutocompleteParseException(Properties.Resources.noBracket + ": >");
                    string pattern = para.Substring(1, closeBracketIndex - 1);
                    switch (pattern)
                    {
                        case "number":
                            pattern = @"-?\d+(\.\d+)?";
                            break;
                        case "coor":
                            pattern = @"(~?(-?\d+(\.\d+)?)|~)";
                            break;
                        case "int":
                            pattern = @"-?\d+";
                            break;
                    }
                    values.Add("^" + pattern + "$");
                    break;
                case '{':
                    type = Type.options;
                    if (!para.Contains("}"))
                        throw new AutocompleteParseException(Properties.Resources.noBracket + ": }");
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
                            throw new AutocompleteParseException(Properties.Resources.noBracket + ": )");
                        int index = para.IndexOf('(');
                        values.Add(para.Substring(1, index - 1));

                        if (!functionNames.Contains(values[0]))
                            throw new AutocompleteParseException(Properties.Resources.unknownFunctionName + ": " + values[0]);
                        foreach (string str in para.Substring(index + 1, para.Length - index - 2).Split(','))
                        {
                            values.Add(str);
                            if (values[0] == "dot")
                                if (!attributes.contains(str))
                                    throw new AutocompleteParseException(Properties.Resources.unknownDotPara + ": " + str);
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
            bool noPrefix = false;
            do
            {
                noPrefix = true;
                foreach (string str in prefix)
                {
                    if (segment.StartsWith(str))
                    {
                        noPrefix = false;
                        segment = segment.Substring(str.Length);
                    }
                }
            } while (!noPrefix);
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
        public bool strictMatch(string segment)
        {
            bool noPrefix = false;
            do
            {
                noPrefix = true;
                foreach (string str in prefix)
                {
                    if (segment.StartsWith(str))
                    {
                        noPrefix = false;
                        segment = segment.Substring(str.Length);
                    }
                }
            } while (!noPrefix);
            switch (type)
            {
                case Type.function:
                    switch (values[0])
                    {
                        case "scbObj":
                            return scbObj.Contains(segment) || runtime_scbObj.Contains(segment);
                        case "trigger":
                            return triggerObj.Contains(segment) || runtime_triggerObj.Contains(segment);
                        case "team":
                            return teams.Contains(segment) || runtime_teams.Contains(segment);
                        case "tag":
                            return tags.Contains(segment) || runtime_tags.Contains(segment);
                        case "sound":
                            return sounds.match(segment);
                        case "dot":
                            var match = false;
                            foreach (string value in values.Skip(1))
                            {
                                if (attributes.match(value + "." + segment))
                                {
                                    match = true;
                                    break;
                                }
                            }
                            return match;
                        case "command":
                            return true;
                        case "selector":
                            if (segment.Length > 2 && (segment.StartsWith("@a") || segment.StartsWith("@e") || segment.StartsWith("@r") || segment.StartsWith("@p")))
                            {
                                if (segment.Length == 2)
                                    return true;
                                if (!(segment[2] == '[' && segment.EndsWith("]")))
                                    return false;
                                segment = segment.Substring(3, segment.Length - 4);
                                if (Regex.IsMatch(segment, @"^,0-9a-zA-Z=!_-"))
                                    return false;
                                var pairs = segment.Split(',');
                                var result = new List<string>();
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
                                foreach (string objective in runtime_scbObj)
                                {
                                    if (result.Contains("score_" + objective))
                                        continue;
                                    result.Add("score_" + objective + "_min");
                                    result.Add("score_" + objective);
                                }
                                foreach (string pair in pairs)
                                {
                                    var key_value = pair.Split('=');
                                    if (key_value.Length > 2)
                                        return false;
                                    if (key_value.Length == 1)
                                    {
                                        if (!Regex.IsMatch(key_value[0], @"^-?\d+$"))
                                            return false;
                                        continue;
                                    }
                                    if (key_value[1].StartsWith("!"))
                                        key_value[1] = key_value[1].Substring(1);
                                    if (Regex.IsMatch(key_value[0], @"[^0-9a-zA-Z_]") || Regex.IsMatch(key_value[1], @"[^0-9a-zA-Z_\-]"))
                                        return false;
                                    if (!result.Contains(key_value[0]))
                                        return false;
                                    result.Remove(key_value[0]);
                                    switch (key_value[0])
                                    {
                                        case "team":
                                            if (key_value[1] != "" && !(teams.Contains(key_value[1]) || runtime_teams.Contains(key_value[1])))
                                                return false;
                                            break;
                                        case "name":
                                            if (key_value[1]!="" && !(names.Contains(key_value[1]) || runtime_names.Contains(key_value[1])))
                                                return false;
                                            break;
                                        case "tag":
                                            if (key_value[1] != "" && !(tags.Contains(key_value[1]) || runtime_tags.Contains(key_value[1])))
                                                return false;
                                            break;
                                        case "type":
                                            if (key_value[1] != "Player" && !getRef("entityID").Contains(key_value[1]))
                                                return false;
                                            break;
                                        default:
                                            if (!Regex.IsMatch(key_value[1], @"^-?\d+$"))
                                                return false;
                                            break;
                                    }
                                }
                                return true;
                            }
                            else
                                return true;
                        case "nbt":
                            return true;
                        default:
                            return false;
                    }
                case Type.reference:                    
                        return (references[values[0]].Contains(segment));                    
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
        public CompletionData getValues(string input)
        {
            List<string> result = new List<string>();
            string beginMatch = input;
            bool noPrefix = false;
            if (forceCompletePrefix)
                foreach (string str in prefix)
                {
                    if (beginMatch.StartsWith(str))
                    {
                        beginMatch = beginMatch.Substring(str.Length);
                    }
                    else
                    {
                        result.Add(str);
                        var result2 = new List<int>();
                        var result3 = new List<bool>();
                        for (int i = 0; i < result.Count; i++)
                        {
                            result2.Add(beginMatch.Length);
                            result3.Add(true);
                        }
                        return new CompletionData(result, result2, result3);
                    }
                }
            else
                do
                {
                    noPrefix = true;
                    foreach (string str in prefix)
                    {
                        if (beginMatch.StartsWith(str))
                        {
                            noPrefix = false;
                            beginMatch = beginMatch.Substring(str.Length);
                        }
                    }
                } while (!noPrefix);
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
                            result.AddRange(scbObj);
                            result.AddRange(runtime_scbObj);
                            break;
                        case "trigger":
                            result.AddRange(triggerObj);
                            result.AddRange(runtime_triggerObj);
                            break;
                        case "team":
                            result.AddRange(teams);
                            result.AddRange(runtime_teams);
                            break;
                        case "tag":
                            result.AddRange(tags);
                            result.AddRange(runtime_tags);
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
                                    result.AddRange(attributes.Autocomplete(value + "." + beginMatch));
                                }
                            beginMatch = input.Split('.').Last();
                            break;
                        case "nbt":
                            beginMatch = input.Split('{', '[', ',').Last();
                            result.AddRange(getRef("nbt"));
                            break;
                        case "selector":
                            if ((beginMatch.StartsWith("@a[") || beginMatch.StartsWith("@p[") || beginMatch.StartsWith("@e[") || beginMatch.StartsWith("@r[")) && beginMatch.LastIndexOf(',') >= beginMatch.LastIndexOf('='))
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
                                foreach (string objective in runtime_scbObj)
                                {
                                    if (result.Contains("score_" + objective))
                                        continue;
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
                                    result = teams;
                                    result.AddRange(runtime_teams);
                                }
                                else if (name == "tag")
                                {
                                    result = tags;
                                    result.AddRange(runtime_tags);
                                }
                                else if (name == "name")
                                {
                                    result = names;
                                    result.AddRange(runtime_names);
                                }
                                beginMatch = input.Split('=', '!').Last();
                            }
                            break;
                    }
                    break;
            }

            var raw = result.Distinct();
            List<string> partMatch = new List<string>();
            List<string> completions = new List<string>();
            bool startswith = false;
            foreach (string s in raw)
            {
                if (s.ToLower().StartsWith(beginMatch.ToLower()))
                    completions.Add(s);
                if (s.ToLower().Contains(beginMatch.ToLower()))
                    partMatch.Add(s);
            }

            if (completions.Count == 0)
            {
                completions.AddRange(partMatch);
            }
            else
            {
                startswith = true;
            }

            completions.Sort();
            List<int> indexes = new List<int>();
            List<bool> startsWith = new List<bool>();
            for (int i = 0; i < completions.Count; i++)
            {
                indexes.Add(beginMatch.Length);
                startsWith.Add(startswith);
            }
            return new CompletionData(completions, indexes, startsWith);
        }
    }
}
