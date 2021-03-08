using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.API.Models
{
    public class SqliteInventoryDbContext : InventoryDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options
            //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
            .UseSqlite("Data Source=inventory.db");
    }
}
