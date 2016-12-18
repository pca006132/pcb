using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.expression
{
    public abstract class Node
    {
        private Node parent = null;
        private Node left = null;
        private Node right = null;
        public bool isLast = false;

        public bool hasChild()
        {
            return left != null || right != null;
        }
        public bool isComplete()
        {
            if (!hasChild())
                return true;
            return left != null && right != null && left.isComplete() && right.isComplete();
        }

        public Node getParent() { return parent; }
        public Node getLeft() { return left; }
        public Node getRight() { return right; }
        public void setLeft(Node node) { left = node; }
        public void setRight(Node node) { right = node; }
        public void setParent(Node node) { parent = node; }

        public abstract string[] toLines(int level);
        public abstract List<string> getCommands();
        public abstract EntityNode getTemp();
        public abstract void setIsLast(List<string[]> fakeplayerseditable);
        public void checkLast()
        {
            setIsLast(new List<string[]>());
    }
}
}
