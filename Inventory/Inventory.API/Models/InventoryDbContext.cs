using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Models
{
    public class InventoryDbContext : DbContext
    {
        public DbSet<InventoryModel> Inventories { get; set; }
        public DbSet<ProductDefinitionModel> ProductDefinitions { get; set; }

    }


}
