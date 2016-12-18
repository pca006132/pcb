using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.expression
{
    public class FakePlayerNode : EntityNode
    {
        public FakePlayerNode(string playerName, string scbObj) : base(playerName, scbObj) { }
        public override void setIsLast(List<string[]> fakeplayerseditable)
        {
            for (int i = fakeplayerseditable.Count - 1; i >= 0; i--)
            {
                string[] param = fakeplayerseditable[i];
                if (param[0] == selector && param[1] == scbObj)
                {
                    isLast = true;
                    fakeplayerseditable.Remove(param);
                }
            }
        }
        public override string[] toLines(int level)
        {
            string currentLine = "";
            for (int i = 0; i < level; i++)
                currentLine += "    ";
            return new string[] { currentLine + selector + " " + scbObj + " " + isLast.ToString() };
        }
    }
}
