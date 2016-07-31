using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace pcb.core.autocomplete
{
    public class TreeNode : IEnumerable
    {
        public static TreeNode generateTree(List<string> options)
        {
            TreeNode tree = new TreeNode("");
            foreach (string text in options)
            {
                string[] keys = text.Split('.');
                TreeNode temp = tree;
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!temp.contains(keys[i]))
                        temp.Add(new TreeNode(keys[i]));
                    temp = temp.GetChild(keys[i]);
                }
            }
            return tree;
        }

        public List<string> Autocomplete(string path)
        {
            string[] keys = path.Split('.');
            TreeNode temp = this;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!temp.contains(keys[i]))
                    return new List<string>();
                else
                {
                    temp = temp.GetChild(keys[i]);
                }
            }
            if (temp.Count != 0)
            {
                List<string> result = new List<string>();
                foreach (var item in temp)
                {
                    result.Add(item.ID);
                }
                return result;
            }
            else
            {
                return new List<string>();
            }
        }
        public bool match(string path)
        {
            string[] keys = path.Split('.');
            TreeNode temp = this;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!temp.contains(keys[i]))
                    return false;
                else
                {
                    temp = temp.GetChild(keys[i]);
                }
            }
            return true;
        }

        private readonly Dictionary<string, TreeNode> _children = new Dictionary<string, TreeNode>();
        public readonly string ID;
        public TreeNode Parent { get; private set; }

        public TreeNode(string id)
        {
            this.ID = id;
        }
        public TreeNode GetChild(string id)
        {
            return this._children[id];
        }
        public void Add(TreeNode item)
        {
            if (item.Parent != null)
            {
                item.Parent._children.Remove(item.ID);
            }

            item.Parent = this;
            this._children.Add(item.ID, item);
        }
        public bool contains(string id)
        {
            if (_children.ContainsKey(id))
                return true;
            else
                return false;
        }
        public IEnumerator<TreeNode> GetEnumerator()
        {
            return this._children.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get { return this._children.Count; }
        }
    }
}
