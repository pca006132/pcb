using Microsoft.VisualStudio.TestTools.UnitTesting;
using pcb.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static pcb.core.CommandBlock.type;

namespace pcb.core.Tests
{
    [TestClass()]
    public class CommandBlockTests
    {
        [TestMethod()]
        public void CommandBlockTest()
        {
            object[][] datas = {
                new object[] {"say \"fa q\" \\","setblock ~0 ~2 ~0 chain_command_block 0 replace {Command:\"say \\\"fa q\\\" \\\\\",auto:1b}",false,(byte)0,ccb},
                new object[] {"cond:say hi","setblock ~0 ~2 ~0 chain_command_block 8 replace {Command:\"say hi\",auto:1b}", true,(byte)0,ccb},
                new object[] {"data:5 icb:say test","setblock ~0 ~2 ~0 command_block 5 replace {Command:\"say test\"}", false, (byte)5, icb},
                new object[] {"data:5 cond:rcb:say hi","setblock ~0 ~2 ~0 repeating_command_block 13 replace {Command:\"say hi\",auto:1b}",true, (byte)5, rcb}
            };
            foreach (object[] data in datas)
            {
                var cb = new CommandBlock((string)data[0], 0, 2, 0, 0, 0);
                Assert.AreEqual(cb.isCond, (bool)data[2]);
                Assert.AreEqual(cb.damage, (byte)data[3]);
                Assert.AreEqual(cb.cbType, (CommandBlock.type)data[4]);
                Assert.AreEqual(cb.ToString(), (string)data[1]);
            }
        }
    }
}