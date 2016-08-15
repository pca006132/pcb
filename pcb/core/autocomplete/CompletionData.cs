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
        public List<bool> startsWith;
        public CompletionData(List<string> display, List<int> indexes)
        {
            displayData = display;
            startLength = indexes;
        }
        public CompletionData(List<string> display, List<int> indexes, List<bool> startswith)
        {
            displayData = display;
            startLength = indexes;
            startsWith = startswith;
        }
        public CompletionData()
        {
            displayData = new List<string>();
            startLength = new List<int>();
            startsWith = new List<bool>();
        }
        public void Filter()
        {
            bool startswith = false;
            List<int> indexes = new List<int>();
            for (int i = displayData.Count -1 ; i >= 0; i--)
            {
                if (startsWith[i])
                    startswith = true;
                else
                    indexes.Add(i);
            }
            if (startswith)
            {
                foreach (int index in indexes)
                {
                    RemoveAt(index);
                }
            }
        }
        public void RemoveAt(int index)
        {
            displayData.RemoveAt(index);
            startLength.RemoveAt(index);
            startsWith.RemoveAt(index);
        }
    }
}
