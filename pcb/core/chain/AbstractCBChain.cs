using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.chain
{
    public abstract class AbstractCBChain : IEnumerable
    {
        protected Stack<CommandBlock> cbStack = new Stack<CommandBlock>();
        protected bool isNewChain = true;
        protected int[] newCoor = new int[3];
        public AbstractCBChain(int[] coor)
        {
            newCoor = coor;
        }

        public abstract void addCb(string cmd, int lineNum);
        public abstract string addSign(string line);
        public abstract AbstractCBChain newChain(int[] coor);
        public abstract List<string> getCommands();

        public IEnumerator GetEnumerator()
        {
            return cbStack.GetEnumerator();
        }
        public int[] getNextCbCoor()
        {
            if (cbStack.Count == 0)
                return newCoor;
            else
                return cbStack.Peek().getNextCbCoor();
        }
        public string condError()
        {
            StringBuilder result = new StringBuilder();
            if (cbStack.Count < 1)
                return "";
            int lastDamage = cbStack.ElementAt(0).damage;
            foreach (CommandBlock cb in cbStack.Reverse())
            {
                if (cb.isCond && cb.damage != lastDamage)
                {
                    result.Append(String.Format("第{0}行的cond CB刚好位于转向的位置\n", cb.lineNum));
                }
                lastDamage = cb.damage;
            }
            return result.ToString();
        }
        //for internal use only
        protected string sign(string cmd, int x, int y, int z, byte direction, bool standing)
        {
            string nbt = cmd.Substring(5);
            return String.Format("setblock ~{0} ~{1} ~{2} {3} {4} replace {5}", x, y, z, (standing ? "standing_sign" :
                    "wall_sign"), direction, nbt);
        }
        protected string sign(string cmd, int x, int y, int z, byte direction)
        {

            return sign(cmd, x, y, z, direction, true);
        }
    }
}
