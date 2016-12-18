using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.expression
{
    public class EntityNode : Node
    {
        public string selector;
        public string scbObj;
        public EntityNode(string selector, string scbObj)
        {
            this.selector = selector;
            this.scbObj = scbObj;
        }
        public EntityNode(string scbObj)
        {
            this.scbObj = scbObj;
        }

        public override List<string> getCommands()
        {
            return new List<string>();
        }
        public override EntityNode getTemp()
        {
            return this;
        }

        public override string[] toLines(int level)
        {
            string currentLine = "";
            for (int i = 0; i < level; i++)
                currentLine += "    ";
            return new string[] { currentLine + selector + " " + scbObj };
        }
        public override void setIsLast(List<string[]> fakeplayerseditable)
        {

        }
    }
}
