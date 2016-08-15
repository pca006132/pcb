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
            Assert.AreEqual("abc", value.getValues("").displayData[0]);

            //check for display
            Assert.AreEqual("abc", value.getValues("a").displayData[0]);
            Assert.AreEqual("abc", value.getValues("test:test2:a").displayData[0]);

            string str = "a";
            CompletionData completionData = value.getValues(str);

            //check for completion
            Assert.AreEqual("abc", str.Remove(str.Length - completionData.startLength[0], completionData.startLength[0]) + completionData.displayData[0]);
            str = "test:a";
            completionData = value.getValues(str);
            Assert.AreEqual("test:abc", str.Remove(str.Length - completionData.startLength[0], completionData.startLength[0]) + completionData.displayData[0]);

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
            CollectionAssert.AreEquivalent(new List<string> {"abc", "abcd", "cdefg"}, value.getValues("").displayData);
            CollectionAssert.AreEquivalent(new List<string> { "abc", "abcd", "cdefg" }, value.getValues("test:").displayData);

            //check for display
            CollectionAssert.AreEquivalent(new List<string> {"abc", "abcd"}, value.getValues("abc").displayData);
            CollectionAssert.AreEquivalent(new List<string> { "abc", "abcd" }, value.getValues("test:abc").displayData);

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
            CollectionAssert.AreEquivalent(new List<string> { "Zombie","Slime", "Skeleton"}, value.getValues("").displayData);
            CollectionAssert.AreEquivalent(new List<string> { "Zombie", "Slime", "Skeleton"}, value.getValues("test:").displayData);

            //check for display
            CollectionAssert.AreEquivalent(new List<string> { "Slime", "Skeleton" }, value.getValues("S").displayData);
            CollectionAssert.AreEquivalent(new List<string> { "Slime", "Skeleton" }, value.getValues("test:S").displayData);
            //check for completion
            //CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("S")[1]);
            //CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("test:S")[1]);

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

            CollectionAssert.AreEquivalent(new List<string> { "stat", "test" }, value.getValues("").displayData);
            CollectionAssert.AreEquivalent(new List<string> { "Zombie", "Slime", "Skeleton", "faQ" }, value.getValues("stat.").displayData);
            //CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("stat.S")[1]);
        }
        [TestMethod]
        public void functionSelectorTest()
        {
            Value value = new Value("$selector");

            CollectionAssert.AreEquivalent(new List<string> {
                "x","y","z","dx","dy","dz","r","rm","c","m","l","lm","team","name","rx","rxm","ry","rym","type","tag",
                "score_a_min","score_a"
            }, value.getValues("@e[").displayData);
            CollectionAssert.AreEquivalent(new List<string> {
                "x","y","z","dx","dy","dz","r","rm","m","l","lm","team","name","rx","rxm","ry","rym","type","tag",
                "score_a_min","score_a"
            }, value.getValues("@e[c=1,").displayData);
            CollectionAssert.AreEquivalent(new List<string> {"Zombie","Slime","Skeleton","Player" }, value.getValues("@e[r=1,type=").displayData);

            //CollectionAssert.AreEquivalent(new List<string> {"","m","x","xm","y","ym" }, value.getValues("@e[r")[1]);
            //CollectionAssert.AreEquivalent(new List<string> { "lime", "keleton" }, value.getValues("@e[type=!S")[1]);
        }


        //match test
        [TestMethod]
        public void selectorTest()
        {
            Value value = new Value("$selector");
            Assert.AreEqual(true, value.strictMatch("@a"));
            Assert.AreEqual(false, value.strictMatch("@a[score_abc=5]"));
        }
        [TestMethod]
        public void optionsTest()
        {
            Value value = new Value("{a|b|c}");
            Assert.AreEqual(true, value.strictMatch("a"));
            Assert.AreEqual(true, value.strictMatch("b"));
            Assert.AreEqual(true, value.strictMatch("c"));
            Assert.AreEqual(false, value.strictMatch("d"));
        }
    }
}
