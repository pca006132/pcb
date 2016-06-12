using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete
{
    public class Tree : IEnumerable
    {
        private static readonly string[] defaultPrefixes = { "icb:", "rcb:", "cond:", "data:", "init:", "after:" };

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

        public List<string>[] autocomplete(string input)
        {
            bool deletePrefix = true;
            if (input.StartsWith(defaultPrefixes[4]))
            {
                input = input.Substring(5);
                deletePrefix = false;
            } else if (input.StartsWith(defaultPrefixes[5]))
            {
                input = input.Substring(6);
                deletePrefix = false;
            }
            while (deletePrefix)
            {
                deletePrefix = false;
                foreach (string prefix in defaultPrefixes.Take(4))
                {
                    if (input.StartsWith(prefix))
                    {
                        int length = prefix.Length;
                        deletePrefix = true;
                        if (prefix == "data:")
                            length = 7;
                        if (input.Length > 7)
                            input = input.Substring(length);
                        else if (input.Length == 7)
                            input = "";
                        else
                            return new List<string>[] { new List<string>(), new List<string>() };
                    }
                }
            }

            string[] keys = input.Split(' ');
            Tree temp = this;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!temp.contains(keys[i]))
                    return new List<string>[] {new List<string>(), new List<string>() };
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
                List<string>[] result = {new List<string>(), new List<string>() };
                foreach (Tree tree in temp)
                {
                    List<string>[] tempList = tree.value.getValues(keys.Last());
                    if (tree.value.type == Value.Type.function && tree.value.values[0] == "command")
                    {
                        nodes.ForEach((t) => {
                            List<string>[] tempList2 = t.value.getValues(keys.Last());
                            tempList[0].AddRange(tempList2[0]);
                            tempList[1].AddRange(tempList2[1]);
                        });
                    }
                    result[0].AddRange(tempList[0]);
                    result[1].AddRange(tempList[1]);
                }
                return result;
            }
            else
            {
                return new List<string>[] { new List<string>(), new List<string>() };
            }
        }
        List<Tree> nodes = new List<Tree>();
        public Value value;
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
