namespace MemStoreDotNet.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using MemStoreDotNet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StorePrunerTests
    {
        public static readonly Guid TestKey = Guid.Parse("ed9c0ce2-0988-4735-89e6-3011fcf2b150");
        public static readonly string TestValue = "Test Value 1";
        public static readonly Person person = new Person();

        public class Person
        {
            public string Name = "Marc";
        }

        /// <summary>
        /// WARNING!!! This is a very long running test to ensure expirations.
        /// </summary>
        //[Ignore] 
        [TestMethod]
        public void StorePruningTest()
        {
            Store.Instance.AddItem("Name", "Marc");
            Store.Instance.AddItem(12, "Twelve", 1);
            Store.Instance.AddItem(TestKey, new Person(), 2);
            Store.Instance.AddItem(person, "Person Marc", 3);
            Debug.WriteLine("Store is initialized.");

            AssureAllStoreItemsExist();
            AssureStoreItemsHaveBeenPruned();
        }

        private void AssureAllStoreItemsExist()
        {
            string name;
            if (Store.Instance.TryGetValue("Name", out name))
            {
                if (name != "Marc")
                {
                    Assert.Fail("Could not get string key");
                }

                Debug.WriteLine("String key was read.");
            }
            else
            {
                Assert.Fail("Could not get string key");
            }

            string number;
            if (Store.Instance.TryGetValue(12, out number))
            {
                if (number != "Twelve")
                {
                    Assert.Fail("Could not get numeric key");
                }

                Debug.WriteLine("Numeric key was read.");
            }
            else
            {
                Assert.Fail("Could not get numeric key");
            }

            Person p;
            if (Store.Instance.TryGetValue(TestKey, out p))
            {
                if (p.Name != "Marc")
                {
                    Assert.Fail("Could not get object value");
                }

                Debug.WriteLine("Object value was read.");
            }
            else
            {
                Assert.Fail("Could not get object value");
            }

            string stringPerson;
            if (Store.Instance.TryGetValue(person, out stringPerson))
            {
                if (stringPerson != "Person Marc")
                {
                    Assert.Fail("Could not get object key");
                }

                Debug.WriteLine("Object key was read.");
            }
            else
            {
                Assert.Fail("Could not get object key");
            }
        }

        private void AssureStoreItemsHaveBeenPruned()
        {
            string number;
            ManualResetEvent signal = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(x =>
            {
                Console.WriteLine("Sleeping 70 seconds.");
                Thread.Sleep(70000);

                if (Store.Instance.TryGetValue(12, out number))
                {
                    Assert.Fail("Key should be expired");
                }
                else
                {
                    Debug.WriteLine("Numeric key expired.");
                }
                signal.Set();
            });
            signal.WaitOne();
            signal.Reset();

            Person p;
            ThreadPool.QueueUserWorkItem(x =>
            {
                Thread.Sleep(60000);
                if (Store.Instance.TryGetValue(TestKey, out p))
                {
                    Assert.Fail("Key should be expired");
                }
                else
                {
                    Debug.WriteLine("Object value expired");
                }
                signal.Set();
            });
            signal.WaitOne();
            signal.Reset();

            string stringPerson;
            ThreadPool.QueueUserWorkItem(x =>
            {
                Thread.Sleep(60000);
                if (Store.Instance.TryGetValue(person, out stringPerson))
                {
                    Assert.Fail("Key should be expired");
                }
                else
                {
                    Debug.WriteLine("Object key expired.");
                }
                signal.Set();
            });
            signal.WaitOne();
            signal.Reset();

            string name;
            ThreadPool.QueueUserWorkItem(x =>
            {
                Thread.Sleep(60000);
                if (Store.Instance.TryGetValue("Name", out name))
                {
                    if (name != "Marc")
                    {
                        Assert.Fail("Could not get string key");
                    }
                }
                else
                {
                    Assert.Fail("Object should not expire.");
                }
                signal.Set();
            });
            signal.WaitOne();
        }
    }
}
