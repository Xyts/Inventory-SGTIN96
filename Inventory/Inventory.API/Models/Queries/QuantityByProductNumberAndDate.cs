using System;

namespace Inventory.API.Models.Query
{
    public class QuantityByProductNumberAndDate
    {
        public DateTime DateOfInventory { get; set; }
        public long CompanyPrefix { get; set; }
        public int ItemReference { get; set; }
        public int Count { get; set; }
    }
}
