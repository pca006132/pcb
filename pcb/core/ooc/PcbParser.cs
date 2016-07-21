using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using pcb.core.chain;
using pcb.core.util;
using System.Text.RegularExpressions;

namespace pcb.core
{
    public class PcbParser
    {
        private static Regex define = new Regex(@"^define (\S+) = (.+)$", RegexOptions.Compiled);

        public static bool markerType = false;
        private int currentLineNum = 0;
        private Stack<AbstractCBChain> chains = new Stack<AbstractCBChain>();
        public string checkForCondDir()
        {
            StringBuilder sb = new StringBuilder();
            foreach (AbstractCBChain chain in chains)
            {
                sb.Append(chain.condError());
            }
            return sb.ToString();
        }
        public int startLine = 0;
        public int endLine = -1;

        //statistics
        public int entityNum = 0;
        public int cbNum = 0;
        public int staticCommandNum = 0;
        public int singleLineCommentNum = 0;
        public int docCommentNum = 0;
        public int moduleNum = 1;

        public string[] getOOC(string pcb, AbstractCBChain chain)
        {
            chains = new Stack<AbstractCBChain>();
            return command2OOC(pcb2Command(pcb, chain));
        }
        
        private string[] pcb2Command(string pcb, AbstractCBChain chain)
        {
            currentLineNum = 0;
            Dictionary<string, string> variables = new Dictionary<string, string>();
            string[] lines = pcb.Split(new string[] {"\r\n", "\n","\r" }, StringSplitOptions.None);
            List<string> initCmd = new List<string>();
            List<string> cbCmd = new List<string>();
            List<string> middleCmd = new List<string>();
            List<string> lastCmd = new List<string>();
            bool isComment = false;
            chains.Push(chain);
            foreach (string rawLine in lines)
            {
                currentLineNum++;
                string line = rawLine.Trim(' ', '\t', '\r');
                if (define.IsMatch(line)) {
                    var regex = define.Match(line);
                    var key = regex.Groups[1].ToString();
                    var value = regex.Groups[2].ToString();
                    if (variables.ContainsKey(key))
                        variables.Remove(key);
                    variables.Add(key, value);
                    continue;
                }
                //multiline comment
                if (isComment)
                {
                    if (line.EndsWith("*/"))
                        isComment = false;
                    continue;
                }
                if (line.StartsWith("/*"))
                {
                    isComment = true;
                    docCommentNum++;
                    continue;
                }
                //single line comment
                if (line.StartsWith("//"))
                {
                    singleLineCommentNum++;
                    continue;
                }

                //empty line
                if (line.Length == 0)
                    continue;

                foreach (var key in variables.Keys)
                {
                    line = line.Replace(key, variables[key]);
                }

                //new
                if (line.StartsWith("new"))
                {
                    string[] components = line.Split(' ');
                    if (components.Length < 4)
                        throw new PcbException(currentLineNum, Properties.Resources.newFormatError);
                    try
                    {
                        int[] newChainCoor = new int[3];
                        newChainCoor[0] = int.Parse(components[1]) + 2;
                        newChainCoor[1] = int.Parse(components[2]) - 2;
                        newChainCoor[2] = int.Parse(components[3]);
                        if (chains.Peek() is BoxCbChain)
                        {
                            newChainCoor[1] += 1;
                            newChainCoor[2] += 1;
                        }
                        if (components.Length == 5 && components[4].Equals("py"))
                        {
                            chains.Push(new StraightCbChain(newChainCoor));
                            ((StraightCbChain)chains.Peek()).disableAutoChangeDirection();
                        }
                        else
                            chains.Push(chain.newChain(newChainCoor));
                    }
                    catch (FormatException)
                    {
                        throw new PcbException(currentLineNum, Properties.Resources.newCoorError);
                    }
                    moduleNum++;
                    continue;
                }
                if (line.StartsWith("init:"))
                {
                    if (currentLineNum >= startLine && (endLine == -1 || currentLineNum <= endLine))
                    {
                        initCmd.Add(line.Substring(5));
                        staticCommandNum++;
                    }
                }
                else if (line.StartsWith("after:"))
                {
                    if (currentLineNum >= startLine && (endLine == -1 || currentLineNum <= endLine))
                    {
                        lastCmd.Add(line.Substring(6));
                        staticCommandNum++;
                    }
                }
                else if (line.StartsWith("mark:"))
                {
                    if (currentLineNum >= startLine && (endLine == -1 || currentLineNum <= endLine))
                    {
                        middleCmd.Add(marker(line, chains.Peek().getNextCbCoor()));
                        entityNum++;
                    }
                }
                else if (line.StartsWith("sign:"))
                {
                    string cmd = chains.Peek().addSign(line);
                    if (currentLineNum >= startLine && (endLine == -1 || currentLineNum <= endLine))
                        if (cmd.Length > 0)
                            middleCmd.Add(cmd);
                }
                else if (line.StartsWith("stats:"))
                {
                    int[] coor = chains.Peek().getNextCbCoor();
                    if (currentLineNum >= startLine && (endLine == -1 || currentLineNum <= endLine))
                    {
                        lastCmd.AddRange(stats(line, coor[0], coor[1], coor[2]));
                        staticCommandNum += 2;
                    }
                }
                else
                    chains.Peek().addCb(line, currentLineNum);
            }
            foreach (AbstractCBChain c in chains)
            {
                cbCmd.AddRange(c.getCommands(startLine, endLine));
            }
            cbNum = cbCmd.Count;
            List<string> result = new List<string>();
            result.AddRange(initCmd);
            result.AddRange(cbCmd);
            result.AddRange(middleCmd);
            result.AddRange(lastCmd);
            return result.ToArray<string>();
        }
        private string[] command2OOC(string[] commands)
        {
            OOCMaker oocs = new OOCMaker(commands);
            return oocs.getOOCs();
        }

        private string marker(string cmd, int[] coor)
        {
            Marker entity;
            string[] parts = cmd.Split(' ');
            string name = parts[0].Substring(5);
            if (markerType)
            {
                entity = new Marker("ArmorStand", name, coor, parts.Skip(1).ToArray());
            }
            else
            {
                entity = new Marker("AreaEffectCloud", name, coor, parts.Skip(1).ToArray());
            }
            return entity.ToString();
        }
        private string[] stats(string cmd, int x, int y, int z)
        {
            string [] components = cmd.Split(' ');
            if (components.Length < 3)
                throw new PcbException(currentLineNum, Properties.Resources.statsFormatError);
            string stat = components[0].Substring(6);
            switch (stat) {
                case "ab":
                    stat = "AffectedBlocks";
                    break;
                case "ae":
                    stat = "AffectedEntities";
                    break;
                case "ai":
                    stat = "AffectedItems";
                    break;
                case "qr":
                    stat = "QueryResult";
                    break;
                case "sc":
                    stat = "SuccessCount";
                    break;
            }
            string[] result = {
                String.Format("scoreboard players set {0} {1} 0",components[1], components[2]),
                String.Format("stats block ~{0} ~{1} ~{2} set {3} {4} {5}", x, y, z, stat, components[1], components[2])
            };
            return result;
        }
    }
}
