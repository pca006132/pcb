using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pcb.core.util;

namespace pcb.core.chain
{
    class StraightCbChain : AbstractCBChain
    {
        public static Direction initialDir = Direction.positiveY;
        private Direction dir = initialDir;
        public static int limit = 0;
        private bool autoChangeDirection = true;
        private int rowCbCount = 0;

        public StraightCbChain(int[] coor) : base(coor) { }

        public override void addCb(string cmd, int lineNum)
        {
            if (cmd.Equals("changeD"))
            {
                if (dir != Direction.positiveX && dir != Direction.negativeX)
                    cbStack.Peek().setDirection(CoorUtil.directionToCBDamage(Direction.positiveX));
                else
                    cbStack.Peek().setDirection(CoorUtil.directionToCBDamage(Direction.positiveZ));
                dir = CoorUtil.inverseDirection(dir);
                return;
            }

            Direction tempDir = dir;
            if (limit != 0 && autoChangeDirection)
            {
                rowCbCount++;
                if (rowCbCount == limit)
                {
                    if (dir != Direction.positiveX && dir != Direction.negativeX)
                        tempDir = Direction.positiveX;
                    else
                        tempDir = Direction.positiveZ;
                    rowCbCount = 0;
                    dir = CoorUtil.inverseDirection(dir);
                }
            }
            int[] coor = getNextCbCoor();
            if (isNewChain)
            {
                isNewChain = false;
                coor = newCoor;
            }
            cbStack.Push(new CommandBlock(cmd, coor[0], coor[1], coor[2],
                    CoorUtil.directionToCBDamage(tempDir), lineNum));
        }

        public override string addSign(string line)
        {
            int[] coor = cbStack.Peek().getNextCbCoor();
            if (dir == Direction.negativeY || dir == Direction.positiveY)
                return (sign(line, coor[0] + 1, coor[1], coor[2], 5, false));
            else
                return (sign(line, coor[0], coor[1] + 1, coor[2], 12));
        }

        public override AbstractCBChain newChain(int[] coor)
        {
            return new StraightCbChain(coor);
        }

        public override List<string> getCommands(int start, int end)
        {
            List<string> cbCmd = new List<string>();
            List<string> rcbCmd = new List<string>();
            foreach (CommandBlock cb in cbStack)
            {
                if (cb.lineNum >= start && (end == -1 || cb.lineNum <= end))
                    if (cb.cbType != CommandBlock.type.rcb)
                    {                    
                        cbCmd.Add(cb.ToString());
                    }
                    else {
                        rcbCmd.Add(cb.ToString());
                    }
            }
            cbCmd.AddRange(rcbCmd);
            return cbCmd;
        }

        public static void setDirection(Direction direction)
        {
            initialDir = direction;
        }
        public static void setRowCbLimit(int count)
        {
            if (count < 2)
                throw new ArgumentException();
            limit = count;
        }
        public void disableAutoChangeDirection()
        {
            autoChangeDirection = false;
        }
}
}
