using System;

namespace InventorySystem
{
    /// <summary>
    /// Event arguments for inventory notifications
    /// </summary>
    public class InventoryNotificationEventArgs : EventArgs
    {
        #region Properties

        public string Label { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for InventoryNotificationEventArgs
        /// </summary>
        /// <param name="item">The item id related to the event</param>
        public InventoryNotificationEventArgs(string label)
        {
            Label = label;
        }

        #endregion
    }
}
