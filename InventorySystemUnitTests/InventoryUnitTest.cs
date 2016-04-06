using InventorySystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace InventorySystemUnitTests
{
    [TestClass]
    public class InventoryUnitTest
    {
        #region Properties

        private IInventory Inventory { get; set; }

        #endregion

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            Inventory = new Inventory(1000);
        }

        [TestMethod]
        public void VerifyAddingItem()
        {
            Assert.IsTrue(Inventory.Add(new InventoryItem("item1", "type1", DateTime.Now.AddDays(1))));
            Assert.IsNotNull(Inventory.Retrieve("item1"));
        }        

        [TestMethod]
        public void VerifyRemovingItem()
        {
            bool eventRaised = false;

            Assert.IsTrue(Inventory.Add(new InventoryItem("item2", "type2", DateTime.Now.AddDays(1))));
            
            Inventory.Removed += delegate(object sender, InventoryNotificationEventArgs e)
            {
                Assert.IsTrue(e.Label == "item2");
                eventRaised = true;
            };

            Assert.IsTrue(Inventory.Remove("item2"));
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void VerifyRemovingNonExistentItem()
        {
            bool eventRaised = false;

            Inventory.Removed += delegate(object sender, InventoryNotificationEventArgs e)
            {
                eventRaised = true;
            };

            Assert.IsFalse(Inventory.Remove("item3"));
            Assert.IsFalse(eventRaised);
        }        

        [TestMethod]
        public void VerifyExpirationNotification()
        {
            bool eventRaised = false;

            Inventory.Expired += delegate(object sender, InventoryNotificationEventArgs e)
            {
                Assert.IsTrue(e.Label == "item3");
                eventRaised = true;
            };

            Assert.IsTrue(Inventory.Add(new InventoryItem("item3", "type3", DateTime.Now.AddSeconds(1))));
            Thread.Sleep(2000);
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void VerifyMultipleExpirationNotifications()
        {
            int events = 0;
            string[] labels = {"item4", "item5", "item6", "item7"};

            Inventory.Expired += delegate(object sender, InventoryNotificationEventArgs e)
            {
                Assert.IsTrue(labels.Contains(e.Label));
                events++;
            };

            Assert.IsTrue(Inventory.Add(new InventoryItem("item4", "type4", DateTime.Now.AddSeconds(1))));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item5", "type5", DateTime.Now.AddSeconds(2))));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item6", "type6", DateTime.Now.AddSeconds(3))));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item7", "type7", DateTime.Now.AddSeconds(4))));
            
            Thread.Sleep(5000);
            Assert.IsTrue(events == labels.Length);
        }

        [TestMethod]
        public void VerifyRemovingMultipleItems()
        {
            int events = 0;
            string[] labels = { "item8", "item9", "item10", "item11" };

            Inventory.Removed += delegate(object sender, InventoryNotificationEventArgs e)
            {
                Assert.IsTrue(labels.Contains(e.Label));
                events++;
            };

            Assert.IsTrue(Inventory.Add(new InventoryItem("item8", "type8", DateTime.Now.AddSeconds(1))));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item9", "type9", DateTime.Now.AddSeconds(2))));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item10", "type10", DateTime.Now.AddSeconds(3))));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item11", "type11", DateTime.Now.AddSeconds(4))));

            Assert.IsTrue(Inventory.Remove("item8"));
            Assert.IsTrue(Inventory.Remove("item9"));
            Assert.IsTrue(Inventory.Remove("item10"));
            Assert.IsTrue(Inventory.Remove("item11"));

            Assert.IsTrue(events == labels.Length);
        }

        [TestMethod]
        public void VerifyAddingExistingItem()
        {            
            Assert.IsTrue(Inventory.Add(new InventoryItem("item12", "type12", DateTime.Now.AddDays(1))));
            int count = Inventory.Count;
            Assert.IsFalse(Inventory.Add(new InventoryItem("item12", "type12", DateTime.Now.AddDays(1))));
            Assert.IsTrue(Inventory.Count == count);
        }

        [TestMethod]
        public void VerifyMultipleExpirationNotificationsAtSameTime()
        {
            int events = 0;
            string[] labels = { "item13", "item14", "item15", "item16" };

            Inventory.Expired += delegate(object sender, InventoryNotificationEventArgs e)
            {
                Assert.IsTrue(labels.Contains(e.Label));
                events++;
            };

            DateTime expirationDate = DateTime.Now.AddSeconds(5);

            Assert.IsTrue(Inventory.Add(new InventoryItem("item13", "type13", expirationDate)));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item14", "type14", expirationDate)));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item15", "type15", expirationDate)));
            Assert.IsTrue(Inventory.Add(new InventoryItem("item16", "type16", expirationDate)));

            Thread.Sleep(7000);
            Assert.IsTrue(events == labels.Length);
        }

        [TestMethod]
        public void VerifyExistingItemRetrieval()
        {
            Assert.IsTrue(Inventory.Add(new InventoryItem("item17", "type17", DateTime.Now.AddDays(1))));
            InventoryItem item = Inventory.Retrieve("item17");
            Assert.IsNotNull(item);
            Assert.IsTrue(item.Label == "item17");            
        }

        [TestMethod]
        public void VerifyNonExistentItemRetrieval()
        {
            InventoryItem item = Inventory.Retrieve("item18");
            Assert.IsNull(item);           
        }

        [TestMethod]
        public void VerifyCreatingItemWithoutLabel()
        {
            bool exceptionReceived = false;

            try
            {
                InventoryItem item = new InventoryItem("", "type1", DateTime.Now.AddDays(1));
            }
            catch(ArgumentException)
            {
                exceptionReceived = true;
            }

            Assert.IsTrue(exceptionReceived);
        }

        [TestMethod]
        public void VerifyAddingItemWithPastExpiration()
        {
            bool eventRaised = false;

            Inventory.Expired += delegate(object sender, InventoryNotificationEventArgs e)
            {
                Assert.IsTrue(e.Label == "item19");
                eventRaised = true;
            };

            Assert.IsTrue(Inventory.Add(new InventoryItem("item19", "type19", DateTime.Now.AddDays(-1))));
            Thread.Sleep(2000);
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void VerifyInventoryInitializationWithInvalidInterval()
        {
            bool exceptionReceived = false;

            try
            {
                IInventory inventory = new Inventory(999);                
            }
            catch (ArgumentException)
            {
                exceptionReceived = true;
            }

            Assert.IsTrue(exceptionReceived);
        } 

        #endregion

    }
}
