using System;

namespace Inventory.API.Models.Queries
{
    public class ItemDetails
    {
        public string InventoryId { get; set; }
        public string InventoryLocation { get; set; }
        public DateTime DateOfInventory { get; set; }
        public long SerialNumber { get; set; }
        public long CompanyPrefix { get; set; }
        public string CompanyName { get; set; }
        public int ItemReference { get; set; }
        public string ProductName { get; set; }
    }
}
