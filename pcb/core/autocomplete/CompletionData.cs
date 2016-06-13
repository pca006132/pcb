using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete
{
    public class CompletionData
    {
        public List<string> displayData;
        public List<int> startLength;
        public CompletionData(List<string> display, List<int> indexes)
        {
            displayData = display;
            startLength = indexes;
        }
        public CompletionData()
        {
            displayData = new List<string>();
            startLength = new List<int>();
        }
    }
}
