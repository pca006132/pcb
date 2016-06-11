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
    public class TagIntTests
    {
        string nbt = "abc:123,abc:-456,xyz:123456789123";
        int index = 0;
        TagInt tagInt;
        TagInt tagInt2;
        TagInt tagInt3;

        public TagIntTests()
        {
            tagInt = TagInt.parseNBT(nbt, ref index);
            tagInt2 = TagInt.parseNBT(nbt, ref index);
            tagInt3 = TagInt.parseNBT(nbt, ref index);
        }
        [TestMethod()]
        public void parseNBTTest()
        {            
            Assert.AreEqual(123, tagInt.value);
            Assert.AreEqual(-456, tagInt2.value);
        }

        [TestMethod()]
        public void EqualsTest()
        {            
            Assert.IsFalse(tagInt.Equals(tagInt2));
        }
    }
}