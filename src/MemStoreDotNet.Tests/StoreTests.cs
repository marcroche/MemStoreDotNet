namespace MemStoreDotNet.Tests
{
    using MemStoreDotNet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StoreTests
    {
        [TestMethod]
        public void AddItemTest()
        {
            Store.Instance.AddItem(1, "One");
            Store.Instance.AddItem(1, 1);

            if (!Store.Instance.ContainsKey<int, string>(1))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ContainsKeyTest()
        {
            Store.Instance.AddItem(1, "One");

            if (!Store.Instance.ContainsKey<int, string>(1))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetItemTest()
        {
            Store.Instance.AddItem(1, "One");

            string value;
            if (Store.Instance.TryGetValue(1, out value))
            {
                if (value != "One")
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemoveItemTest()
        {
            Store.Instance.AddItem(1, "One");

            Store.Instance.RemoveItem<int, string>(1);

            string value;
            if (Store.Instance.TryGetValue(1, out value))
            {
                Assert.Fail();
            }
        }
    }
}
