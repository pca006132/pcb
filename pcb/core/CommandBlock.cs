using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pcb.core.util;

namespace pcb.core
{
    public class CommandBlock
    {
        public enum type { icb, ccb, rcb}
        private int relativeX;
        private int relativeY;
        private int relativeZ;
        public int lineNum;
        private bool _isCond = false;
        public bool isCond
        {
            private set
            {
                _isCond = value;
            }
            get
            {
                return _isCond;
            }
        }
        private bool isAuto = true;
        private byte _damage;
        public byte damage
        {
            private set
            {
                _damage = value;
            }
            get
            {
                return _damage;
            }
        }
        public type cbType = type.ccb;
        private string command;
        public CommandBlock(string cmd, int x, int y, int z, byte direction, int lineNum)
        {
            relativeX = x;
            relativeY = y;
            relativeZ = z;
            this.lineNum = lineNum;
            damage = direction;
            bool havePrefix = true;
            do
            {
                if (cmd.StartsWith("icb:"))
                {
                    cbType = type.icb;
                    isAuto = false;
                    cmd = cmd.Substring(4);
                } else if (cmd.StartsWith("rcb:"))
                {
                    cbType = type.rcb;
                    cmd = cmd.Substring(4);
                } else if (cmd.StartsWith("cond:"))
                {
                    isCond = true;
                    cmd = cmd.Substring(5);
                } else if (cmd.StartsWith("data:"))
                {
                    try
                    {
                        damage = Byte.Parse(cmd.Substring(5, 1));
                        if (damage > 7)
                            throw new PcbException(this.lineNum, Properties.Resources.dataLargerThan7);
                        if (cmd[6] != ' ')
                            throw new PcbException(this.lineNum, Properties.Resources.dataFormatError);
                        cmd = cmd.Substring(7);
                    } catch (FormatException)
                    {
                        throw new PcbException(this.lineNum, Properties.Resources.dataNotNum);
                    }
                } else
                {
                    havePrefix = false;
                    command = cmd;
                }                
            } while (havePrefix);
        }
        public void setDirection(byte direction)
        {
            damage = direction;
        }
        public int[] getNextCbCoor()
        {
            int[] coor = { relativeX, relativeY, relativeZ };
            return CoorUtil.move(coor, CoorUtil.CBDamageToDirection(damage));
        }
        public override string ToString()
        {
            string block = "";
            switch (cbType)
            {
                case type.rcb:
                    block = "repeating_command_block";
                    break;
                case type.ccb:
                    block = "chain_command_block";
                    break;
                case type.icb:
                    block = "command_block";
                    break;
            }
            return String.Format("setblock ~{0} ~{1} ~{2} {3} {4} replace {{Command:\"{5}\"{6}}}", relativeX, relativeY, relativeZ, block, damage + (isCond ? 8 : 0), CommandUtil.escape(command), (isAuto ? ",auto:1b" : ""));
        }
    }
}
