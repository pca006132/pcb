using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.expression
{
    public class TempNode : FakePlayerNode
    {
        private static HashSet<int> temps = new HashSet<int>();
        public TempNode() : base("temp", "temps")
        {
            int temp = 0;
            while (temps.Contains(temp))
            {
                temp++;
            }
            temps.Add(temp);
            this.selector = "temp" + temp.ToString();
            isLast = true;
        }
    }
}
