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
            string jsonString;
            try
            {
                jsonString = File.ReadAllText("ref/references.json");
            } catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.ioError + " references.json: \n" + ex.Message);
            }
            JObject json;
            try
            {
                json = JObject.Parse(jsonString);
            } catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.formatError + ": references.json: \n" + ex.Message);
            }            
            foreach (var pair in json)
            {
                if (pair.Value.Type == JTokenType.Array)
                    Value.addRef(pair.Key, ((JArray)pair.Value).Select(s => (string)s).ToList());
            }

            try
            {
                jsonString = File.ReadAllText("ref/dot.json");
            }
            catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.ioError + " dot.json: \n" + ex.Message);
            }
            try
            {
                json = JObject.Parse(jsonString);
            }
            catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.formatError + ": dot.json: \n" + ex.Message);
            }
            foreach (var pair in json)
            {
                if (pair.Value.Type == JTokenType.Array)
                    Value.addAttributes(pair.Key, ((JArray)pair.Value).Select(s => (string)s).ToList());
            }            

            try
            {
                jsonString = File.ReadAllText("ref/sounds.json");
            }
            catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.ioError + " sounds.json: \n" + ex.Message);
            }
            try
            {
                Value.setSoundJson(jsonString);
            }
            catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.formatError + ": sounds.json: \n" + ex.Message);
            }
            string commands;
            try
            {
                commands = File.ReadAllText("ref/commands.txt");
            } catch (Exception ex)
            {
                throw new AutocompleteParseException(Properties.Resources.ioError + " commands.txt: \n" + ex.Message);
            }
            return Tree.generateTree(commands);
        }
    }
}
