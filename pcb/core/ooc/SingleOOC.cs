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
        static string prefix = "/summon FallingSand ~ ~1.5 ~ " +
                "{Time:1,Block:minecraft:redstone_block,Motion:[0d,-1d,0d],Passengers:" +
                "[{id:FallingSand,Time:1,Block:minecraft:activator_rail" +
                ",Passengers:[";
        static string suffix = "{id:MinecartCommandBlock,Command:" +
                "setblock ~ ~1 ~ command_block 0 replace {auto:1b,Command:fill ~ ~ ~ ~ ~-2 ~ air}},{id:MinecartCommandBlock,Command:kill @e[type=MinecartCommandBlock,r=1]}]}]}";
        static string cmdPrefix = "{id:MinecartCommandBlock,Command:";
        static int prefixLength = prefix.Length;
        static int colorPrefixLength = getColorModeLength(prefix, false);
        static int cmdPrefixLength = cmdPrefix.Length;
        static int colorCmdPrefixLength = getColorModeLength(cmdPrefix, false);
        static int suffixLength = suffix.Length;
        static int colorSuffixLength = getColorModeLength(suffix, false);
        static int colorSignLength = colorBlackTech("").Length;
        public static int oocLimit = 31000;
        static bool version_1_11 = false;
        //end constants
        
        private bool useColorBlackTech;
        private StringBuilder cmd = new StringBuilder();
        private int normalLength = prefixLength;
        private int colorModeLength = colorPrefixLength;
        private bool oocGenerated = false;

        public SingleOOC()
        {
            if (version_1_11 != PcbParser.version_1_11) {
                version_1_11 = PcbParser.version_1_11;
                if (version_1_11)
                {
                    prefix = "/summon falling_block ~ ~1.5 ~ " +
                        "{Time:1,Block:minecraft:redstone_block,Motion:[0d,-1d,0d],Passengers:" +
                        "[{id:falling_block,Time:1,Block:minecraft:activator_rail" +
                        ",Passengers:[";
                    suffix = "{id:commandblock_minecart,Command:" +
                        "setblock ~ ~1 ~ command_block 0 replace {auto:1b,Command:fill ~ ~ ~ ~ ~-2 ~ air}},{id:commandblock_minecart,Command:kill @e[type=commandblock_minecart,r=1]}]}]}";
                    cmdPrefix = "{id:commandblock_minecart,Command:";

                    prefixLength = prefix.Length;
                    colorPrefixLength = getColorModeLength(prefix, false);
                    cmdPrefixLength = cmdPrefix.Length;
                    colorCmdPrefixLength = getColorModeLength(cmdPrefix, false);
                    suffixLength = suffix.Length;
                    colorSuffixLength = getColorModeLength(suffix, false);
                    colorSignLength = colorBlackTech("").Length;
                } else
                {
                    prefix = "/summon FallingSand ~ ~1.5 ~ " +
                "{Time:1,Block:minecraft:redstone_block,Motion:[0d,-1d,0d],Passengers:" +
                "[{id:FallingSand,Time:1,Block:minecraft:activator_rail" +
                ",Passengers:[";
                    suffix = "{id:MinecartCommandBlock,Command:" +
                            "setblock ~ ~1 ~ command_block 0 replace {auto:1b,Command:fill ~ ~ ~ ~ ~-2 ~ air}},{id:MinecartCommandBlock,Command:kill @e[type=MinecartCommandBlock,r=1]}]}]}";
                    cmdPrefix = "{id:MinecartCommandBlock,Command:";
                    prefixLength = prefix.Length;
                    colorPrefixLength = getColorModeLength(prefix, false);
                    cmdPrefixLength = cmdPrefix.Length;
                    colorCmdPrefixLength = getColorModeLength(cmdPrefix, false);
                    suffixLength = suffix.Length;
                    colorSuffixLength = getColorModeLength(suffix, false);
                    colorSignLength = colorBlackTech("").Length;
                }
            }

            cmd.Append(prefix);
            addCommand("blockdata ~ ~-2 ~ {auto:0b,Command:\"\"}");
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
                colorModeLength += getColorModeLength(command, true) + colorCmdPrefixLength + 6;
                normalLength += getEscapedLength(command) + cmdPrefixLength + 4;
            }
            else
            {
                cmd.Append(command);
                colorModeLength += getColorModeLength(command, false) + colorCmdPrefixLength + 2;
                normalLength += command.Length + cmdPrefixLength + 2;
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
                if (getEscapedLength(command) > oocLimit)
                    throw new PcbException(Properties.Resources.commandTooLong);
                if (useColorBlackTech || command.Contains("§"))
                {
                    if (colorModeLength + getColorModeLength(command, true) + colorCmdPrefixLength + 6 + colorSuffixLength + colorSignLength> oocLimit)
                        return false;
                    else
                        return true;
                }
                else {
                    if (normalLength + getEscapedLength(command) + cmdPrefixLength + 4 + suffixLength > oocLimit)
                        return false;
                    else
                        return true;
                }
            } else
            {
                if (command.Length > oocLimit)
                    throw new PcbException(Properties.Resources.commandTooLong);
                if (useColorBlackTech || command.Contains("§"))
                {
                    if (colorModeLength + getColorModeLength(command, false) + colorCmdPrefixLength + 2 + colorSuffixLength + colorSignLength > oocLimit)
                        return false;
                    else
                        return true;
                }
                else {
                    if (normalLength + command.Length + cmdPrefixLength + 2 + suffixLength > oocLimit)
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
                return specialCharCount * 7 + strLength + colorCharCount;
            else
                return specialCharCount * 3 + strLength + colorCharCount;
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
                if (version_1_11)
                    cmd.Insert(23,"-1");
                else
                    cmd.Insert(21, "-1");
                cmd.Append(suffix);
                oocGenerated = true;
                return colorBlackTech(cmd.ToString()).Insert(10,"1");
            }
        }
    }
}
