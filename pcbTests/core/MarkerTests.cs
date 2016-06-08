using Microsoft.VisualStudio.TestTools.UnitTesting;
using pcb.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.Tests
{
    [TestClass()]
    public class MarkerTests
    {
        [TestMethod()]
        public void MarkerArmorStandTest()
        {
            Marker marker = new Marker("ArmorStand", "test", new int[] {1,2,3 }, new string[] { });
            Assert.AreEqual("summon ArmorStand ~1 ~2 ~3 {CustomName:test,NoGravity:1b,Invisible:1b}", marker.ToString());

            marker = new Marker("ArmorStand", "test", new int[] { 1, 2, 3 }, new string[] {"tag1"});
            Assert.AreEqual("summon ArmorStand ~1 ~2 ~3 {CustomName:test,NoGravity:1b,Invisible:1b,Tags:[tag1]}",
                marker.ToString());

            marker = new Marker("ArmorStand", "test", new int[] { 1, 2, 3 }, new string[] { "tag1","tag2" });
            Assert.AreEqual("summon ArmorStand ~1 ~2 ~3 {CustomName:test,NoGravity:1b,Invisible:1b,Tags:[tag1,tag2]}",
                marker.ToString());
        }
        [TestMethod]
        public void MarkerAECTest()
        {
            Marker marker = new Marker("AreaEffectCloud", "test", new int[] { 1, 2, 3 }, new string[] { });
            Assert.AreEqual("summon AreaEffectCloud ~1 ~2 ~3 {CustomName:test,Duration:2147483647}", marker.ToString());

            marker = new Marker("AreaEffectCloud", "test", new int[] { 1, 2, 3 }, new string[] { "tag1" });
            Assert.AreEqual("summon AreaEffectCloud ~1 ~2 ~3 {CustomName:test,Duration:2147483647,Tags:[tag1]}",
                marker.ToString());

            marker = new Marker("AreaEffectCloud", "test", new int[] { 1, 2, 3 }, new string[] { "tag1", "tag2" });
            Assert.AreEqual("summon AreaEffectCloud ~1 ~2 ~3 {CustomName:test,Duration:2147483647,Tags:[tag1,tag2]}",
                marker.ToString());
        }
        [TestMethod]
        [ExpectedException(typeof(PcbException))]
        public void MarkerCoorErrorTest()
        {
            Marker marker = new Marker("AreaEffectCloud", "test", new int[] { 1, 2}, new string[] { });
        }
        [TestMethod]
        [ExpectedException(typeof(PcbException))]
        public void MarkerTypeErrorTest()
        {
            Marker marker = new Marker("AreaEffectClou", "test", new int[] { 1, 2, 3 }, new string[] { });
        }

    }
}