using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Newtonsoft.Json.Linq;

namespace pcb.core.autocomplete
{
    public class InitAutocomplete
    {
        public static Tree init()
        {
            string jsonString = File.ReadAllText("ref/references.json");
            JObject json = JObject.Parse(jsonString);
            foreach (var pair in json)
            {
                if (pair.Value.Type == JTokenType.Array)
                    Value.addRef(pair.Key, ((JArray)pair.Value).Select(s => (string)s).ToList());
            }

            jsonString = File.ReadAllText("ref/dot.json");
            json = JObject.Parse(jsonString);
            foreach (var pair in json)
            {
                if (pair.Value.Type == JTokenType.Array)
                    Value.addAttributes(pair.Key, ((JArray)pair.Value).Select(s => (string)s).ToList());
            }            

            string commands = File.ReadAllText("ref/commands.txt");
            return Tree.generateTree(commands);
        }
    }
}
