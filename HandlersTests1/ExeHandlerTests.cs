using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;


namespace Handlers.Tests
{
    [TestClass()]
    public class ExeHandlerTests
    {
        [TestMethod()]
        public void CheckExeDoesntExist()
        {
            ExeHandler exeHandler = new ExeHandler();
            MemoryManager memoryManager = new MemoryManager();
            memoryManager.MarkTabAsOpen("Cheese", "ud");
            exeHandler.RunExe("che", "12");
            Assert.ThrowsException<Win32Exception>(() => exeHandler.RunExe(@"G:\WWW\NOT HERE.exe", "12"));
            // Assert.ThrowsException<Win32Exception>(()=> ExeHandler.RunExe(@"G:\My Drive\Computer Science\Exam POOP\Topic Test Paper 2 ANSWERS"));

        }
    }
}
