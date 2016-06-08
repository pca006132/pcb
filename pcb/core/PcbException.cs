using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core
{
    [Serializable]
    public class PcbException : Exception
    {
        public PcbException() : base("unknown exception")
        {
        }
        public PcbException(string message) : base(message)
        {            
        }
        public PcbException(int lineNum, string message) : base(String.Format("line {0}:\n{1}",lineNum,message))
        {
        }
    }
}
