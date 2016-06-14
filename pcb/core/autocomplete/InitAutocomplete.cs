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
                throw new AutocompleteParseException("未能读取references.json: \n" + ex.Message);
            }
            JObject json;
            try
            {
                json = JObject.Parse(jsonString);
            } catch (Exception ex)
            {
                throw new AutocompleteParseException("references.json格式错误: \n" + ex.Message);
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
                throw new AutocompleteParseException("未能读取dot.json: \n" + ex.Message);
            }
            try
            {
                json = JObject.Parse(jsonString);
            }
            catch (Exception ex)
            {
                throw new AutocompleteParseException("dot.json格式错误: \n" + ex.Message);
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
                throw new AutocompleteParseException("未能读取sounds.json: \n" + ex.Message);
            }
            try
            {
                Value.setSoundJson(jsonString);
            }
            catch (Exception ex)
            {
                throw new AutocompleteParseException("sounds.json格式错误: \n" + ex.Message);
            }
            string commands;
            try
            {
                commands = File.ReadAllText("ref/commands.txt");
            } catch (Exception ex)
            {
                throw new AutocompleteParseException("未能读取commands.txt: \n" + ex.Message);
            }
            return Tree.generateTree(commands);
        }
    }
}
