using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete
{
    public class Tree : IEnumerable
    {
        public static Tree generateTree(string text)
        {
            Tree tree = new Tree(null);
            foreach (string rawLine in text.Split('\n'))
            {
                string line = rawLine.Trim();
                Tree temp = tree;
                foreach (string para in line.Split(' '))
                {
                    if (!temp.containsRaw(para))
                        temp.addNode(new Tree(para));
                    temp = temp.getChildRaw(para);
                }
            }
            return tree;
        }

        public List<string>[] autocomplete(string input)
        {
            string[] keys = input.Split(' ');
            Tree temp = this;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!temp.contains(keys[i]))
                    return new List<string>[] {new List<string>(), new List<string>() };
                else
                {
                    temp = temp.getChild(keys[i]);
                }
            }
            if (temp.Count > 0)
            {
                List<string>[] result = {new List<string>(), new List<string>() };
                foreach (Tree tree in temp)
                {
                    List<string>[] tempList = tree.value.getValues(keys.Last());
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
            foreach (Tree node in nodes)
            {
                if (node.value.isMatch(para))
                    return node;
            }
            return null;
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
