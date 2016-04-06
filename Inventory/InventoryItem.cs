using System;

namespace InventorySystem
{
    /// <summary>
    /// Represents an item in an inventory
    /// </summary>
    public class InventoryItem
    {       
        #region Properties

        /// <summary>
        /// The expiration date for the item
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// The label for the item
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The type of the item
        /// </summary>
        public string Type { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for InventoryItem
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="type">The type of the item</param>
        /// <param name="expiration">The expiration date</param>
        public InventoryItem(string label, string type, DateTime expiration)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentException("Label must be specified.");
            }

            Label = label;
            Type = type;
            Expiration = expiration.ToUniversalTime();
        }

        #endregion
    }
}
