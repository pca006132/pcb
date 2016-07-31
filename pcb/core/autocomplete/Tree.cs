using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete
{
    public class Tree : IEnumerable
    {
        private static readonly string[] defaultPrefixes = { "icb:", "rcb:", "cond:", "data:"};
        private static readonly string[] prefixes = { "init:", "after:" };
        public static List<string> defines = new List<string>();

        public static Tree generateTree(string text)
        {
            Tree tree = new Tree(null);                  

            foreach (string rawLine in text.Split('\n'))
            {
                string line = rawLine.Trim();
                if (line.Length == 0)
                    continue;
                Tree temp = tree;
                foreach (string para in line.Split(' '))
                {
                    try
                    {
                        if (!temp.containsRaw(para))
                            temp.addNode(new Tree(para));
                        temp = temp.getChildRaw(para);
                    } catch (AutocompleteParseException ex)
                    {
                        throw new AutocompleteParseException(ex.Message + "\nat: " + para + "\nat: " + line);
                    }
                }
            }
            return tree;
        }

        public CompletionData autocomplete(string input)
        {
            string[] keys = input.Split(' ');

            CompletionData result = new CompletionData();
            
            if (prefixes[0].StartsWith(keys[0]) || keys[0].StartsWith(prefixes[0]))
            {
                if (keys.Length == 1 && prefixes[0].Length > keys[0].Length)
                {
                    result.displayData.Add(prefixes[0]);
                    result.startLength.Add(keys[0].Length);
                } else
                {
                    keys[0] = keys[0].Substring(prefixes[0].Length);
                }
            }
            if (prefixes[1].StartsWith(keys[0]) || keys[0].StartsWith(prefixes[1]))
            {
                if (keys.Length == 1 && prefixes[1].Length > keys[0].Length)
                {
                    result.displayData.Add(prefixes[1]);
                    result.startLength.Add(keys[0].Length);
                }
                else
                    keys[0] = keys[0].Substring(prefixes[1].Length);
            }
            
            List<string> posiblePrefixes = new List<string>(defaultPrefixes);
            bool notFinish = true;
            while (notFinish)
            {
                notFinish = false;
                foreach (string prefix in posiblePrefixes)
                {
                    if (keys[0].StartsWith(prefix))
                    {
                        if (prefix == "data:")
                        {
                            if (7 < keys[0].Length)
                                keys[0] = keys[0].Substring(7);
                            else
                                keys[0] = "";
                        }
                        else
                        {
                            if (prefix.Length < keys[0].Length)
                                keys[0] = keys[0].Substring(prefix.Length);
                            else
                                keys[0] = "";
                        }
                        posiblePrefixes.Remove(prefix);
                        notFinish = true;
                        break;
                    }
                    else if (keys.Length == 1 && prefix.StartsWith(keys[0]) && prefix.Length > keys[0].Length)
                    {
                        result.displayData.Add(prefix);
                        result.startLength.Add(keys[0].Length);
                    }
                }
            }

            if (input.Length == 0)
            {
                return new CompletionData();
            }
            if (keys[0].StartsWith("/"))
                keys[0] = keys[0].Substring(1);

            Tree temp = this;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!temp.contains(keys[i]))
                    return new CompletionData();
                else
                {
                    temp = temp.getChild(keys[i]);
                    if (temp.value.type == Value.Type.function && temp.value.values[0] == "command")
                    {
                        temp = this;
                        i--;
                    }
                }
            }
            if (temp.Count > 0)
            {                
                foreach (Tree tree in temp)
                {
                    CompletionData tempList = tree.value.getValues(keys.Last());
                    if (tree.value.type == Value.Type.function && tree.value.values[0] == "command")
                    {
                        nodes.ForEach((t) => {
                            CompletionData tempList2 = t.value.getValues(keys.Last());
                            tempList.displayData.AddRange(tempList2.displayData);
                            tempList.startLength.AddRange(tempList2.startLength);
                        });
                    }
                    result.displayData.AddRange(tempList.displayData);
                    result.startLength.AddRange(tempList.startLength);
                }
                foreach (string define in defines)
                {
                    if (define.ToLower().StartsWith(keys.Last().ToLower()))
                    {
                        result.displayData.Add(define);
                        result.startLength.Add(keys.Last().Length);
                    }
                }
                return result;
            }
            else
            {
                return new CompletionData();
            }
        }
        public int check(string line)
        {
            string[] keys = line.Split(' ');
            if (keys[0].StartsWith("//"))
                return -1;
            var key0 = keys[0];
            if (keys[0].StartsWith("mark:") || keys[0].StartsWith("stats:") || keys[0].StartsWith("new") || keys[0].StartsWith("sign") || keys[0].StartsWith("changeD"))
                return -1;
            if (prefixes[0].StartsWith(keys[0]) || keys[0].StartsWith(prefixes[0]))
            {
                if (keys.Length == 1 && prefixes[0].Length > keys[0].Length)
                {
                    return 0;
                }
                else
                {
                    if (keys[0].Length > prefixes[0].Length)
                        keys[0] = keys[0].Substring(prefixes[0].Length);
                    else
                        keys[0] = "";
                }                           
            }
            if (prefixes[1].StartsWith(keys[0]) || keys[0].StartsWith(prefixes[1]))
            {
                if (keys.Length == 1 && prefixes[1].Length > keys[0].Length)
                {
                    return 0;
                }
                else
                {
                    if (keys[0].Length > prefixes[1].Length)
                        keys[0] = keys[0].Substring(prefixes[1].Length);
                    else
                        keys[0] = "";
                }
            }

            List<string> posiblePrefixes = new List<string>(defaultPrefixes);
            bool notFinish = true;
            while (notFinish)
            {
                notFinish = false;
                foreach (string prefix in posiblePrefixes)
                {
                    if (keys[0].StartsWith(prefix))
                    {
                        if (prefix == "data:")
                        {
                            if (7 < keys[0].Length)
                                keys[0] = keys[0].Substring(7);
                            else
                                keys[0] = "";
                        }
                        else
                        {
                            if (prefix.Length < keys[0].Length)
                                keys[0] = keys[0].Substring(prefix.Length);
                            else if (keys.Length == 1)
                                return -1;
                            else
                                keys[0] = "";
                        }
                        posiblePrefixes.Remove(prefix);
                        notFinish = true;
                        break;
                    }
                }
            }

            if (keys[0].StartsWith("/"))
                keys[0] = keys[0].Substring(1);
            Tree temp = this;

            for (int i = 0; i < keys.Length; i++)
            {
                if (temp.Count == 0)
                    break;
                if (!temp.strictContains(keys[i]))
                {
                    int offset = 0;
                    for (int j = 0; j < i; j++)
                    {
                        if (j == 0)
                            offset += key0.Length;
                        else
                            offset += keys[j].Length;
                        offset += 1;
                    }
                    return offset;
                }
                else
                {
                    temp = temp.getChild(keys[i]);
                    if (temp.value.type == Value.Type.function && temp.value.values[0] == "command")
                    {
                        temp = this;
                        if (keys[i].StartsWith("/"))
                            keys[i] = keys[i].Substring(1);
                        i--;
                    }
                }
            }
            return -1;
        }

        List<Tree> nodes = new List<Tree>();
        public Value value = null;
        public Tree(string str)
        {
            if (str != null)
                this.value = new Value(str);
        }
        public void addNode(Tree node)
        {
            if (!nodes.Contains(node))
                nodes.Add(node);
        }
        public bool contains(string para)
        {
            foreach (Tree node in nodes)
            {
                if (node.value.isMatch(para))
                    return true;
            }
            return false;
        }
        public bool strictContains(string para)
        {
            foreach (Tree node in nodes)
            {
                if (node.value.strictMatch(para))
                    return true;
            }
            return false;
        }
        public bool containsRaw(string para)
        {
            foreach (Tree node in nodes)
                if (node.value.rawValue.Equals(para))
                    return true;
            return false;
        }
        public Tree getChild(string para)
        {
            Tree func = null;
            foreach (Tree node in nodes)
            {
                if (node.value.isMatch(para))
                {
                    if (node.value.type == Value.Type.function)
                        func = node;
                    else
                        return node;
                }
            }
            return func;
        }
        public Tree getChildStrict(string para)
        {
            Tree func = null;
            foreach (Tree node in nodes)
            {
                if (node.value.strictMatch(para))
                {
                    if (node.value.type == Value.Type.function)
                        func = node;
                    else
                        return node;
                }
            }
            return func;
        }
        public Tree getChildRaw(string para)
        {
            foreach (Tree node in nodes)
                if (node.value.rawValue.Equals(para))
                    return node;
            return null;
        }
        public IEnumerator GetEnumerator()
        {
            return nodes.GetEnumerator();            
        }
        public int Count
        {
            get { return this.nodes.Count; }
        }
    }
}
