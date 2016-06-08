using Microsoft.VisualStudio.TestTools.UnitTesting;
using static pcb.core.util.Direction;
using static pcb.core.util.CoorUtil;
using pcb.core.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.util.Tests
{
    [TestClass()]
    public class Coor_UtilTests
    {
        [TestMethod()]
        public void directionToCBDamageTest()
        {
            Assert.AreEqual(5, directionToCBDamage(positiveX));
            Assert.AreEqual(4, directionToCBDamage(negativeX));
            Assert.AreEqual(3, directionToCBDamage(positiveZ));
            Assert.AreEqual(2, directionToCBDamage(negativeZ));
            Assert.AreEqual(1, directionToCBDamage(positiveY));
            Assert.AreEqual(0, directionToCBDamage(negativeY));
        }

        [TestMethod()]
        public void CBDamageToDirectionTest()
        {
            Assert.AreEqual(Direction.positiveX, CoorUtil.CBDamageToDirection(5));
            Assert.AreEqual(Direction.negativeX, CoorUtil.CBDamageToDirection(4));
            Assert.AreEqual(Direction.positiveZ, CoorUtil.CBDamageToDirection(3));
            Assert.AreEqual(Direction.negativeZ, CoorUtil.CBDamageToDirection(2));
            Assert.AreEqual(Direction.positiveY, CoorUtil.CBDamageToDirection(1));
            Assert.AreEqual(Direction.negativeY, CoorUtil.CBDamageToDirection(0));
        }

        [TestMethod()]
        public void moveTest()
        {
            int[] coor = { 1, 2, 3 };
            CollectionAssert.AreEqual(new int[] { 2, 2, 3 }, move(coor, positiveX));
            CollectionAssert.AreEqual(new int[] { 0, 2, 3 }, move(coor, negativeX));
            CollectionAssert.AreEqual(new int[] { 1, 2, 4 }, move(coor, positiveZ));
            CollectionAssert.AreEqual(new int[] { 1, 2, 2 }, move(coor, negativeZ));
            CollectionAssert.AreEqual(new int[] { 1, 3, 3 }, move(coor, positiveY));
            CollectionAssert.AreEqual(new int[] { 1, 1, 3 }, move(coor, negativeY));
        }

        [TestMethod()]
        public void inverseDirectionTest()
        {
            Assert.AreEqual(negativeX, inverseDirection(positiveX));
            Assert.AreEqual(positiveX, inverseDirection(negativeX));
            Assert.AreEqual(negativeY, inverseDirection(positiveY));
            Assert.AreEqual(positiveY, inverseDirection(negativeY));
            Assert.AreEqual(negativeZ, inverseDirection(positiveZ));
            Assert.AreEqual(positiveZ, inverseDirection(negativeZ));
        }
    }
}