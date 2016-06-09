using Microsoft.VisualStudio.TestTools.UnitTesting;
using pcb.core.autocomplete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete.Tests
{
    [TestClass()]
    public class TreeTests
    {
        Tree tree;
        public TreeTests()
        {
            pcbTests.core.autocomplete.InitStaticValue.init();
            tree = Tree.generateTree(@"scoreboard objectives add <\w+>'记分板名称 $dot(stat)'判据 <\w+>'记分板显示名称" + "\n" +
                @"scoreboard objectives remove $$scbObj'记分板名称" + "\n" +
                @"scoreboard objectives setdisplay #displaySlot'显示位置 $scbObj'记分板名称" + "\n" +
                @"scoreboard objectives list" + "\n" +
                @"scoreboard players {set|add|remove|reset} $selector'实体或假名 $scbObj'记分板名称 <-?\d+>'分数");
        }
        [TestMethod]
        public void autocompleteTests()
        {
            CollectionAssert.AreEquivalent(new List<string> { "scoreboard" }, tree.autocomplete("")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "objectives","players" }, tree.autocomplete("scoreboard ")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "set", "add","remove","reset" }, 
                tree.autocomplete("scoreboard players ")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "a" },
                tree.autocomplete("scoreboard players add @a ")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "stat","dummy","trigger" },
                tree.autocomplete("scoreboard objectives add abc ")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "killEntity", "animalsBred", "drop" },
                tree.autocomplete("scoreboard objectives add abc stat.")[0]);
            CollectionAssert.AreEquivalent(new List<string> {"Zombie", "Slime", "Skeleton"}, 
                tree.autocomplete("scoreboard objectives add abc stat.killEntity.")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "Zombie", "Slime", "Skeleton" ,"Player"},
                tree.autocomplete("scoreboard players add @e[type=!")[0]);
        }
        [TestMethod]
        public void containsRawTest()
        {
            Tree tree = new Tree(null);
            tree.addNode(new Tree("test"));
            tree.addNode(new Tree("#displaySlot"));
            Assert.IsTrue(tree.containsRaw("test"));
            Assert.IsTrue(tree.containsRaw("#displaySlot"));
        }
    }
}
