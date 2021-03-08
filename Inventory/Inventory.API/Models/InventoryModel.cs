using System;

namespace Inventory.API.Models
{
    public class InventoryModel
    {
        public int Id { get; set; }
        public string InventoryId { get; set; }
        public string InventoryLocation { get; set; }
        public DateTime DateOfInventory { get; set; }
        public long SerialNumber { get; set; }

        public int ProductDefinitionId { get; set; }
        public ProductDefinitionModel ProductDefinition { get; set; }
    }
}
