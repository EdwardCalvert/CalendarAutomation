using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Handlers.Tests
{
    [TestClass()]
    public class URLHandlerTests
    {
        [TestMethod()]
        public void CheckValidURL()
        {

            Assert.IsTrue(URLHandler.IsValidUri("https://jimdo.com"));
            Assert.IsFalse(URLHandler.IsValidUri("cheese"));
            Assert.IsTrue(URLHandler.IsValidUri("https://192.168.1.1"));
        }

    }
}