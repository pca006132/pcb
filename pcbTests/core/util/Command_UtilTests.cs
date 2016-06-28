using Microsoft.VisualStudio.TestTools.UnitTesting;
using static pcb.core.util.CommandUtil;
using pcb.core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.util.Tests
{
    [TestClass()]
    public class Command_UtilTests
    {
        [TestMethod()]
        public void escapeTest()
        {
            string testString = "\"\\ \\\\ \\\\\\ ";
            Assert.AreEqual("\\\"\\\\ \\\\\\\\ \\\\\\\\\\\\ ", escape(testString));
            Assert.AreEqual(testString, unescape(escape(testString)));
        }

        [TestMethod()]
        public void colorBlackTechTest()
        {
            Assert.AreEqual(@"setblock ~ ~ ~ standing_sign 0 replace {Text1:""{\""text\"":\""right click\"",\""clickEvent\"":{\""action\"":\""run_command\"",\""value\"":\""\u00a7a\\\""\""}}"",Text2:""{\""text\"":\""\""}"",Text3:""{\""text\"":\""\""}"",Text4:""{\""text\"":\""\""}""}",
                colorBlackTech("§a\""));
        }
    }
}
