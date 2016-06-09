using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pcb.core.autocomplete;

namespace pcbTests.core.autocomplete
{
    public class InitStaticValue
    {
        static bool inited = false;
        public static void init()
        {
            if (inited)
                return;
            Value.addRef("entityID", new List<string> { "Zombie", "Slime", "Skeleton" });
            Value.addAttributes("att", new List<string> { "stat.#entityID", "stat.faQ" });
            Value.addAttributes("test", new List<string> { "test.hiU","test.faq" });
            Value.addAttributes("test2", new List<string> { "test2.wtf" });
            Value.addAttributes("stat", new List<string> { "stat.killEntity.#entityID","stat.animalsBred","stat.drop",
                "dummy","trigger" });
            Value.addRef("displaySlot", new List<string> { "sidebar", "list" });
            Value.scbObj.Add("a");
            inited = true;
        }
    }
}
