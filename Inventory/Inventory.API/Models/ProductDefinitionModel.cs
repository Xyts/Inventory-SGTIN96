using System.Collections.Generic;

namespace Inventory.API.Models
{
    public class ProductDefinitionModel
    {
        public int Id { get; set; }
        public long CompanyPrefix { get; set; }
        public string CompanyName { get; set; }
        public int ItemReference { get; set; }
        public string ProductName { get; set; }

        public List<InventoryModel> InventoryModels { get; set; }
    }
}
