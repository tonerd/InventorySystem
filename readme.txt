SYNOPSIS

This is a simple inventory system API with the ability to add and remove expirable items, and receive notifications on removal or expiration.

HOW TO RUN

This project contains no executable program or UI, it is only runnable by loading and executing the included unit test project in Visual Studio.

DESIGN

This inventory system was implemented using a dictionary structure where the label of the inventory item is used as a key.  There is a thread running on a 
configurable interval which checks the expiration date of inventory items, removes them when expired, and triggers and event.

API FUNCTIONS

An interface is included with the following functionality:

int Count													//The number of items in the inventory               
bool Add(InventoryItem item)								//Add an item to the inventory        
bool Remove(string label)									//Remove an item from the inventory
InventoryItem Retrieve(string label)						//Retrieve an item from the inventory
event EventHandler<InventoryNotificationEventArgs> Expired  //Event fires when an inventory item expires
event EventHandler<InventoryNotificationEventArgs> Removed	//Event fires when an inventory item is removed

ASSUMPTIONS

1.  Labels for inventory items are unique, and only one item with a particular label can exist in the inventory at one time.
2.  Inventory is stored in memory, which would not happen in real life.
3.  Expired items can be added, but will be removed immediately when the expiration check runs.
4.  Expiration check will occur at most every 1000 milliseconds.  Inventory cannot be initialized with a lower value.
