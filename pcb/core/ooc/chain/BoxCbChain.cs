using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pcb.core.util;

namespace pcb.core.chain
{
    class BoxCbChain : AbstractCBChain
    {
        public static string outerBlock = "stained_glass";
        public static byte outerDamage = 0;
        public static string baseBlock = "quartz_block";
        public static byte baseDamage = 0;

        public static int xLimit = 5;
        public static int zLimit = 5;

        private List<string> signs = new List<string>();
        private int xCount = 0;
        private int yCount = 0;
        private int zCount = 0;
        private int signCount = 0;
        private Direction xDir = Direction.positiveX;
        private Direction zDir = Direction.positiveZ;


        public BoxCbChain(int[] coor) : base(coor) { }

        public void setxLimit(int limit)
        {
            if (limit < 2)
                throw new ArgumentException();
            xLimit = limit;
        }
        public void setzLimit(int limit)
        {
            if (limit < 2)
                throw new ArgumentException();
            zLimit = limit;
        }
        public void setOuterCase(string block, byte damage)
        {
            outerBlock = block;
            outerDamage = damage;
        }
        public void setBaseCase(string block, byte damage)
        {
            baseBlock = block;
            baseDamage = damage;
        }

        public override void addCb(string cmd, int lineNum)
        {
            xCount++;
            Direction tempDir = xDir;
            if (xCount == xLimit)
            {
                xCount = 0;
                xDir = CoorUtil.inverseDirection(xDir);
                zCount++;
                tempDir = zDir;
                if (zCount == zLimit)
                {
                    tempDir = Direction.positiveY;
                    zCount = 0;
                    zDir = CoorUtil.inverseDirection(zDir);
                    yCount++;
                }
            }
            if (isNewChain)
            {
                isNewChain = false;
                cbStack.Push(new CommandBlock(cmd, newCoor[0], newCoor[1], newCoor[2],
                        CoorUtil.directionToCBDamage(tempDir), lineNum));
            }
            else {
                int[] coor = getNextCbCoor();
                cbStack.Push(new CommandBlock(cmd, coor[0], coor[1], coor[2],
                        CoorUtil.directionToCBDamage(tempDir), lineNum));
            }
        }

        public override string addSign(string line)
        {
            signs.Add(line);
            return "";
        }
        private int[] getSignCoor()
        {
            int[] coor = (int[])newCoor.Clone();
            coor[0] -= 2;
            coor[1] = coor[1] + signCount;
            coor[2] = coor[2] + (zLimit / 2);
            return coor;
        }

        public override List<string> getCommands(int start, int end)
        {
            List<string> cb_cmd = new List<string>();
            List<string> rcb_cmd = new List<string>();
            if ((end != -1 && cbStack.Last().lineNum > end) || cbStack.First().lineNum < start)
                return cb_cmd;
            //outer block
            if (start == 0 && end == -1)
            {
                cb_cmd.Add(String.Format(
        "fill ~{0} ~{1} ~{2} ~{3} ~{4} ~{5} {6} {7} hollow", newCoor[0] - 1, newCoor[1], newCoor[2] - 1,
        newCoor[0] + xLimit, newCoor[1] + yCount, newCoor[2] + zLimit,
        outerBlock, outerDamage));
                //top block
                cb_cmd.Add(String.Format(
                        "fill ~{0} ~{1} ~{2} ~{3} ~{4} ~{5} {6} {7} hollow", newCoor[0] - 1, newCoor[1] - 1, newCoor[2] - 1,
                        newCoor[0] + xLimit, newCoor[1] - 1, newCoor[2] + zLimit,
                        baseBlock, baseDamage));
                //base block
                cb_cmd.Add(String.Format(
                        "fill ~{0} ~{1} ~{2} ~{3} ~{4} ~{5} {6} {7} hollow", newCoor[0] - 1, newCoor[1] + yCount + 1,
                        newCoor[2] - 1, newCoor[0] + xLimit, newCoor[1] + yCount + 1, newCoor[2] + zLimit,
                        baseBlock, baseDamage));
            }
            foreach (CommandBlock cb in cbStack)
            {
                if (cb.cbType != CommandBlock.type.rcb)
                {
                    if (cb.lineNum >= start)
                        if (end == -1 || cb.lineNum <= end)
                            cb_cmd.Add(cb.ToString());
                }
                else {
                    if (cb.lineNum >= start)
                        if (end == -1 || cb.lineNum <= end)
                            rcb_cmd.Add(cb.ToString());
                }
            }
            cb_cmd.AddRange(rcb_cmd);
            foreach (string str in signs)
            {
                string nbt = str;
                int[] coor = getSignCoor();
                int[] coor1 = { newCoor[0] - 2 - coor[0], newCoor[1] - 1 - coor[1], newCoor[2] - 1 - coor[2] };
                int[] coor2 = {newCoor[0] + xLimit - coor[0], newCoor[1] + yCount + 1 - coor[1],
                    newCoor[2] + zLimit - coor[2]};

                nbt = nbt.Replace("{delete me}", String.Format(
                        "fill ~{0} ~{1} ~{2} ~{3} ~{4} ~{5} air 0", coor1[0], coor1[1], coor1[2],
                        coor2[0], coor2[1], coor2[2]));
                signCount++;
                cb_cmd.Add(sign(nbt, coor[0], coor[1], coor[2], 4, false));
            }
            return cb_cmd;
        }

        public override AbstractCBChain newChain(int[] coor)
        {
            return new BoxCbChain(coor);
        }
    }
}
