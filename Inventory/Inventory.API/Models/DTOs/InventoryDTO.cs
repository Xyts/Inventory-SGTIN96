using System;
using System.Collections.Generic;

namespace Inventory.API.Models.DTOs
{
    public class InventoryDTO
    {
        public string InventoryId { get; set; }
        public string InventoryLocation { get; set; }
        public DateTime DateOfInventory { get; set; }
        public List<string> SGTIN96Tags { get; set; }
    }
}
