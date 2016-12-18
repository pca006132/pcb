using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.expression
{
    public class ConstantNode : EntityNode
    {
        public ConstantNode(string num) : base(num, "constant")
        {
            constants.Add(num);
        }
        private static HashSet<string> constants = new HashSet<string>();
        public static string[] setupConstants()
        {
            List<string> commands = new List<string>();
            foreach (string constant in constants)
            {
                commands.Add(string.Format("scoreboard players set {0} constant {1}", constant, constant));
            }
            return commands.ToArray();
        }
    }
}
