using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static pcb.core.util.CommandUtil;
namespace pcb.core
{
    public class SingleOOC
    {
        //constants
        const string prefix = "/summon FallingSand ~ ~2 ~ " +
                "{Time:1,Block:minecraft:redstone_block,Passengers:" +
                "[{id:FallingSand,Time:1,Block:minecraft:activator_rail" +
                ",Passengers:[";
        const string suffix = "{id:MinecartCommandBlock,Command:" +
                "setblock ~ ~1 ~ command_block 0 replace {auto:1b,Command:fill ~ ~ ~ ~ ~-2 ~ air}},{id:MinecartCommandBlock,Command:kill @e[type=MinecartCommandBlock,r=1]}]}]}";
        const string cmdPrefix = "{id:MinecartCommandBlock,Command:";
        static int prefixLength = prefix.Length;
        static int colorPrefixLength = getColorModeLength(prefix, false);
        static int cmdPrefixLength = cmdPrefix.Length;
        static int colorCmdPrefixLength = getColorModeLength(cmdPrefix, false);
        static int suffixLength = suffix.Length;
        static int colorSuffixLength = getColorModeLength(suffix, false);
        //end constants

        private bool useColorBlackTech;
        private StringBuilder cmd = new StringBuilder();
        private int normalLength = prefixLength;
        private int colorModeLength = colorPrefixLength;
        private bool oocGenerated = false;

        public SingleOOC()
        {
            cmd.Append(prefix);
        }

        public void addCommand(string command)
        {
            if (oocGenerated)
                throw new InvalidOperationException();
            if (command.Contains("§"))
                useColorBlackTech = true;
            cmd.Append(cmdPrefix);
            if (needEscape(command))
            {
                cmd.Append("\"");
                cmd.Append(escape(command));
                cmd.Append("\"");
                colorModeLength += getColorModeLength(command, true) + colorCmdPrefixLength + 4;
                normalLength += getEscapedLength(command) + cmdPrefixLength + 3;
            }
            else
            {
                cmd.Append(command);
                colorModeLength += getColorModeLength(command, false) + colorCmdPrefixLength + 4;
                normalLength += command.Length + cmdPrefixLength + 1;
            }
            cmd.Append("},");            
        }
        public string getOOC()
        {
            if (useColorBlackTech)
                return getColorOOC();
            else
                return getNormalOOC();
        }
        public bool canAddCommand(string command)
        {
            bool _needEscape = needEscape(command);
            if (_needEscape)
            {
                if (getEscapedLength(command) > 30000)
                    throw new PcbException(Properties.Resources.commandTooLong);
                if (useColorBlackTech || command.Contains("§"))
                {
                    if (colorModeLength + getColorModeLength(command, true) + colorCmdPrefixLength + 4 + colorSuffixLength > 28000)
                        return false;
                    else
                        return true;
                }
                else {
                    if (normalLength + getEscapedLength(command) + cmdPrefixLength + 3 + suffixLength > 30000)
                        return false;
                    else
                        return true;
                }
            } else
            {
                if (command.Length > 30000)
                    throw new PcbException(Properties.Resources.commandTooLong);
                if (useColorBlackTech || command.Contains("§"))
                {
                    if (colorModeLength + getColorModeLength(command, false) + colorCmdPrefixLength + 4 + colorSuffixLength > 28000)
                        return false;
                    else
                        return true;
                }
                else {
                    if (normalLength + command.Length + cmdPrefixLength + 1 + suffixLength > 30000)
                        return false;
                    else
                        return true;
                }
            }
        }

        private static int getEscapedLength(string str)
        {
            int specialCharCount = 0;
            int strLength = str.Length;
            for (int i = 0; i < strLength; i++)
            {
                switch (str[i])
                {
                    case '"':
                    case '\\':
                        specialCharCount++;
                        break;
                }
            }
            return specialCharCount + strLength;
            // (specialCharCount * 2 + non special char count)
        }
        private static int getColorModeLength(string str, bool doubleEscape)
        {
            int specialCharCount = 0;
            int colorCharCount = 0;
            int strLength = str.Length;
            for (int i = 0; i < strLength; i++)
            {
                switch (str[i])
                {
                    case '"':
                    case '\\':
                        specialCharCount++;
                        break;
                    case '§':
                        colorCharCount += 5; //+ 6(-> 6 char) - 1(from str length)
                        break;
                }
            }
            if (doubleEscape)
                return specialCharCount * 3 + strLength + colorCharCount;
            else
                return specialCharCount + strLength + colorCharCount;
        }
        private static bool needEscape(string str)
        {
            Stack<char> brackets = new Stack<char>();
            foreach (char chr in str)
            {
                switch (chr)
                {
                    case '{':
                    case '[':
                        brackets.Push(chr);
                        break;
                    case '}':
                        if (brackets.Count == 0)
                            return true;
                        if (brackets.Peek() == '{')
                            brackets.Pop();
                        else
                            return true;
                        break;
                    case ']':
                        if (brackets.Count == 0)
                            return true;
                        if (brackets.Peek() == ']')
                            brackets.Pop();
                        else
                            return true;
                        break;
                    case ',':
                        if (brackets.Count == 0)
                            return true;
                        break;                       
                }
            }
            if (brackets.Count > 0)
                return true;
            return false;
        }

        string getNormalOOC()
        {
            if (oocGenerated)
                return cmd.ToString();
            else {                
                cmd.Append(suffix);
                oocGenerated = true;
                return cmd.ToString();
            }
        }
        string getColorOOC()
        {
            if (oocGenerated)
                return colorBlackTech(cmd.ToString()).Insert(10,"1");
            else {
                cmd.Insert(21, "-1");
                cmd.Append(suffix);
                oocGenerated = true;
                return colorBlackTech(cmd.ToString()).Insert(10,"1");
            }
        }
    }
}
