using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using pcb.core.autocomplete;
using pcb.core.chain;
using pcb.core.util;

namespace pcb.core
{
    public class LoadConfig
    {
        public static void readConfig()
        {
            Dictionary<string, string> configs = new Dictionary<string, string>();
            foreach (string line in File.ReadAllLines("ref/auto-config.txt"))
            {
                string[] pair = line.Split('=');
                if (pair.Length == 2)
                {
                    string key = pair[0].Trim();
                    string value = pair[1].Trim();
                    if (configs.ContainsKey(key))
                    {
                        configs.Remove(key);
                    }
                    configs.Add(key,value);
                }
            }
            foreach (string key in configs.Keys)
            {
                string value = configs[key];
                switch (key)
                {
                    case "strict-autocomplete":
                        if (value.ToLower() == "true")
                            Value.forceCompletePrefix = true;
                        break;
                    case "lazyMatch-autocomplete":
                        if (value.ToLower() == "false")
                            Value.lazyMatch = false;
                        break;
                }
            }
        }
    }
}
