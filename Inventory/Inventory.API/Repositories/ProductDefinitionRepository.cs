using Inventory.API.Models;
using Inventory.API.Repositories.Interfaces;
using System.Linq;

namespace Inventory.API.Repositories
{
    public class ProductDefinitionRepository : IProductDefinitionRepository
    {
        InventoryDbContext inventoryDbContext;

        public ProductDefinitionRepository(InventoryDbContext inventoryDbContext)
        {
            this.inventoryDbContext = inventoryDbContext;
        }

        public void InsertProductDefinition(ProductDefinitionModel productDefinitionModel)
        {
            inventoryDbContext.ProductDefinitions.Add(productDefinitionModel);
            inventoryDbContext.SaveChanges();
        }

        public ProductDefinitionModel GetProductdefinitionByCompanyPrefixAndReference(long companyPrefix, int reference)
        {
            var query = from product in inventoryDbContext.ProductDefinitions
                        where product.CompanyPrefix == companyPrefix && product.ItemReference == reference
                        select product;

            return query.FirstOrDefault();
        }

        public void DeleteProductByCompanyPrefixAndReference(long companyPrefix, int reference)
        {
            var query = from product in inventoryDbContext.ProductDefinitions
                        where product.CompanyPrefix == companyPrefix && product.ItemReference == reference
                        select product;
            if (query.Any())
            {
                inventoryDbContext.ProductDefinitions.Remove(query.First());
            }
            
            inventoryDbContext.SaveChanges();
        }
    }
}
