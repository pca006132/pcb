using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core
{
    class OOCMaker
    {
        private Stack<SingleOOC> oocs = new Stack<SingleOOC>();
        public OOCMaker(string[] commands)
        {
            oocs.Push(new SingleOOC());
            foreach (string cmd in commands)
            {
                if (oocs.Peek().canAddCommand(cmd))
                    oocs.Peek().addCommand(cmd);
                else
                {
                    oocs.Push(new SingleOOC());
                    oocs.Peek().addCommand(cmd);
                }
            }
        }
        public string[] getOOCs()
        {
            List<string> results = new List<string>();
            foreach (SingleOOC ooc in oocs)
            {
                results.Add(ooc.getOOC());
            }
            return results.ToArray();
        }
    }
}
