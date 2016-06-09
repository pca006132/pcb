using Microsoft.VisualStudio.TestTools.UnitTesting;
using pcb.core.autocomplete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pcb.core.autocomplete.Tests
{
    [TestClass()]
    public class ValueTests
    {        
        public ValueTests()
        {
            pcbTests.core.autocomplete.InitStaticValue.init();
        }
        [TestMethod]
        public void typeNormalTest()
        {
            Value value = new Value("[test:][test2:]abc'fuckU");
            //check for no input
            Assert.AreEqual("abc", value.getValues("")[0][0]);

            //check for display
            Assert.AreEqual("abc", value.getValues("a")[0][0]);
            Assert.AreEqual("abc", value.getValues("test:test2:a")[0][0]);
            //check for completion
            Assert.AreEqual("bc", value.getValues("a")[1][0]);
            Assert.AreEqual("bc", value.getValues("test:a")[1][0]);

            //check for if match
            //is match
            Assert.IsTrue(value.isMatch("abc"));
            Assert.IsTrue(value.isMatch("test:abc"));
            Assert.IsTrue(value.isMatch("test2:test:abc"));
            //not match
            Assert.IsFalse(value.isMatch("a"));
            Assert.IsFalse(value.isMatch("test:a"));
            Assert.IsFalse(value.isMatch("ts:abc"));
        }
        [TestMethod]
        public void typeOptionsTest()
        {
            Value value = new Value("[test:]{abc|abcd|cdefg}'UMotherFucker");
            //check for no input
            CollectionAssert.AreEquivalent(new List<string> {"abc", "abcd", "cdefg"}, value.getValues("")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "abc", "abcd", "cdefg" }, value.getValues("test:")[0]);

            //check for display
            CollectionAssert.AreEquivalent(new List<string> {"abc", "abcd"}, value.getValues("abc")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "abc", "abcd" }, value.getValues("test:abc")[0]);
            //check for completion
            CollectionAssert.AreEquivalent(new List<string> { "", "d" }, value.getValues("abc")[1]);
            CollectionAssert.AreEquivalent(new List<string> { "", "d" }, value.getValues("test:abc")[1]);

            //check for if match
            //is match
            Assert.IsTrue(value.isMatch("abc"));
            Assert.IsTrue(value.isMatch("abcd"));
            Assert.IsTrue(value.isMatch("cdefg"));
            Assert.IsTrue(value.isMatch("test:abc"));
            Assert.IsTrue(value.isMatch("test:abcd"));
            Assert.IsTrue(value.isMatch("test:cdefg"));
            //not match
            Assert.IsFalse(value.isMatch("a"));
            Assert.IsFalse(value.isMatch("test:a"));
            Assert.IsFalse(value.isMatch("te:abc"));
        }
        [TestMethod]
        public void typeRegexTest()
        {
            Value value = new Value("[test:]<coor>'UMotherFucker");            

            //check for if match
            //is match
            Assert.IsTrue(value.isMatch("~"));
            Assert.IsTrue(value.isMatch("~123"));
            Assert.IsTrue(value.isMatch("~-5.456"));
            Assert.IsTrue(value.isMatch("123.456"));
            Assert.IsTrue(value.isMatch("test:~"));
            //not match
            Assert.IsFalse(value.isMatch(""));
            Assert.IsFalse(value.isMatch("~123.456.789"));
            Assert.IsFalse(value.isMatch("te:~"));
        }
        [TestMethod]
        public void typeReferenceTest()
        {
            Value value = new Value("[test:]#entityID'fuckU");
            //check for no input
            value.getValues("")[0].ForEach(s => System.Diagnostics.Debug.Print(s));
            CollectionAssert.AreEquivalent(new List<string> { "Zombie","Slime", "Skeleton"}, value.getValues("")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "Zombie", "Slime", "Skeleton"}, value.getValues("test:")[0]);

            //check for display
            CollectionAssert.AreEquivalent(new List<string> { "Slime", "Skeleton" }, value.getValues("S")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "Slime", "Skeleton" }, value.getValues("test:S")[0]);
            //check for completion
            CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("S")[1]);
            CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("test:S")[1]);

            //check for if match
            //is match
            Assert.IsTrue(value.isMatch("Zombie"));
            Assert.IsTrue(value.isMatch("Slime"));
            Assert.IsTrue(value.isMatch("Skeleton"));
            Assert.IsTrue(value.isMatch("test:Zombie"));
            Assert.IsTrue(value.isMatch("test:Slime"));
            Assert.IsTrue(value.isMatch("test:Skeleton"));
            //not match
            //will always match
            //in lazy mode
        }
        
        [TestMethod]
        public void functionDotTest()
        {
            Value value = new Value("$dot(att,test)");

            CollectionAssert.AreEquivalent(new List<string> { "stat", "test" }, value.getValues("")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "Zombie", "Slime", "Skeleton", "faQ" }, value.getValues("stat.")[0]);
            CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("stat.S")[1]);
        }
        [TestMethod]
        public void functionSelectorTest()
        {
            Value value = new Value("$selector");

            CollectionAssert.AreEquivalent(new List<string> {
                "x","y","z","dx","dy","dz","r","rm","c","m","l","lm","team","name","rx","rxm","ry","rym","type","tag",
                "score_a_min","score_a"
            }, value.getValues("@e[")[0]);
            CollectionAssert.AreEquivalent(new List<string> {
                "x","y","z","dx","dy","dz","r","rm","m","l","lm","team","name","rx","rxm","ry","rym","type","tag",
                "score_a_min","score_a"
            }, value.getValues("@e[c=1,")[0]);
            CollectionAssert.AreEquivalent(new List<string> {"Zombie","Slime","Skeleton","Player" }, value.getValues("@e[r=1,type=")[0]);

            CollectionAssert.AreEquivalent(new List<string> {"","m","x","xm","y","ym" }, value.getValues("@e[r")[1]);
            CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("@e[type=!S")[1]);
        }
    }
}
