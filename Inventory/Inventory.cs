using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InventorySystem
{
    /// <summary>
    /// Represents an inventory
    /// </summary>
    public class Inventory : IInventory, IDisposable
    {
        #region Constants

        const int EXPIRATION_MINIMUM = 1000;

        #endregion

        #region Privates

        private int checkInterval;

        private Dictionary<string, InventoryItem> items;
        private Thread expirationThread;
        
        #endregion

        #region Events

        /// <summary>
        /// Event fires when an inventory item expires
        /// </summary>
        public event EventHandler<InventoryNotificationEventArgs> Expired;

        /// <summary>
        /// Event fires when an inventory item is removed
        /// </summary>
        public event EventHandler<InventoryNotificationEventArgs> Removed;

        #endregion

        #region Properties

        /// <summary>
        /// The number of items in the inventory
        /// </summary>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for inventory
        /// </summary>
        /// <param name="expirationCheckInterval">The interval between checks for expired items in milliseconds</param>
        public Inventory(int expirationCheckInterval)
        {
            if (expirationCheckInterval < EXPIRATION_MINIMUM)
            {
                throw new ArgumentException("Expiration check interval must be at least " + EXPIRATION_MINIMUM); 
            }

            items = new Dictionary<string, InventoryItem>();
            checkInterval = expirationCheckInterval;
            expirationThread = new Thread(new ThreadStart(CheckForExpiredItems));
            expirationThread.Start();
        }        

        #endregion                

        #region Methods

        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if success</returns>
        public bool Add(InventoryItem item)
        {
            if(items.ContainsKey(item.Label))
            {
                return false;
            }

            items.Add(item.Label, item);

            return true;
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            if (expirationThread.IsAlive)
            {
                expirationThread.Abort();
            }
        }

        /// <summary>
        /// Remove an item from the inventory
        /// </summary>
        /// <param name="label">The label of the item to remove</param>
        /// <returns>True if success</returns>
        public bool Remove(string label)
        {            
            bool result = items.Remove(label);

            if(result)
            {
                OnRemoved(new InventoryNotificationEventArgs(label));
            }

            return result;
        }

        /// <summary>
        /// Retrieve an item from the inventory
        /// </summary>
        /// <param name="label">The label fo the item to retrieve</param>
        /// <returns>The inventory item</returns>
        public InventoryItem Retrieve(string label)
        {
            if(items.ContainsKey(label))
            {
                return items[label];
            }
            return null;
        }

        /// <summary>
        /// Invoke the Expired event
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnExpired(InventoryNotificationEventArgs e)
        {
            if (Expired != null)
            {
                Expired(this, e);
            }
        }

        /// <summary>
        /// Invoke the Removed event
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnRemoved(InventoryNotificationEventArgs e)
        {
            if (Removed != null)
            {
                Removed(this, e);
            }
        }

        /// <summary>
        /// Checks for expired items, removes them, and sends a notification event
        /// </summary>
        private void CheckForExpiredItems()
        {
            while (true)
            {                             
                List<string> expiredItemKeys = items.Values.Where(s => s.Expiration <= DateTime.UtcNow).Select(s => s.Label).ToList();
                                           
                foreach (string key in expiredItemKeys)
                {
                    items.Remove(key);
                    OnExpired(new InventoryNotificationEventArgs(key));
                }

                Thread.Sleep(checkInterval);
            }
        }

        #endregion
    }
}
