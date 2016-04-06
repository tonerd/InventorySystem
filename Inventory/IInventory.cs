using System;

namespace InventorySystem
{
    public interface IInventory
    {
        /// <summary>
        /// The number of items in the inventory
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Add an item to the inventory
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <returns>True if success</returns>
        bool Add(InventoryItem item);

        /// <summary>
        /// Remove an item from the inventory
        /// </summary>
        /// <param name="label">The label of the item to remove</param>
        /// <returns>True if success</returns>
        bool Remove(string label);

        /// <summary>
        /// Retrieve an item from the inventory
        /// </summary>
        /// <param name="label">The label fo the item to retrieve</param>
        /// <returns>The inventory item</returns>
        InventoryItem Retrieve(string label);

        /// <summary>
        /// Dispose of resources
        /// </summary>
        void Dispose();

        /// <summary>
        /// Event fires when an inventory item expires
        /// </summary>
        event EventHandler<InventoryNotificationEventArgs> Expired;

        /// <summary>
        /// Event fires when an inventory item is removed
        /// </summary>
        event EventHandler<InventoryNotificationEventArgs> Removed;
    }
}
