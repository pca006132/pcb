using Microsoft.VisualStudio.TestTools.UnitTesting;
using pcbSimulation.nbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcbSimulation.nbt.Tests
{
    [TestClass()]
    public class TagShortTests
    {
        string nbt = "abc:123s,abc:-456s,xyz:123456789123s";
        int index = 0;
        TagShort tagShort;
        TagShort tagShort2;
        TagShort tagShort3;

        public TagShortTests()
        {
            tagShort = TagShort.parseNBT(nbt, ref index);
            tagShort2 = TagShort.parseNBT(nbt, ref index);
            tagShort3 = TagShort.parseNBT(nbt, ref index);
        }
        [TestMethod()]
        public void parseNBTTest()
        {
            Assert.AreEqual(123, tagShort.value);
            Assert.AreEqual(-456, tagShort2.value);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.IsFalse(tagShort.Equals(tagShort2));
        }
    }
}