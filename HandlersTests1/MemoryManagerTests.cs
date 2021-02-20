using Microsoft.VisualStudio.TestTools.UnitTesting;
using Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handlers.Tests
{
    [TestClass()]
    public class MemoryManagerTests
    {
        [TestMethod()]
        public void MarkTabAsOpenTest()
        {
            MemoryManager memoryManager = new MemoryManager();
            Assert.IsFalse(memoryManager.TabOpened("", ""));
            Assert.IsFalse(memoryManager.TabOpened("Cheese.com", ""));
            memoryManager.MarkTabAsOpen("Cheese.com", "12323");
            Assert.IsFalse(memoryManager.TabOpened("Cheese.com", "12645646434"));
            Assert.IsTrue(memoryManager.TabOpened("Cheese.com", "12323"));
        }
    }
}