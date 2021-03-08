using Inventory.API.Models;

namespace Inventory.API.Repositories.Interfaces
{
    public interface IProductDefinitionRepository
    {
        void InsertProductDefinition(ProductDefinitionModel productDefinitionModel);
        ProductDefinitionModel GetProductdefinitionByCompanyPrefixAndReference(long companyPrefix, int reference);
        void DeleteProductByCompanyPrefixAndReference(long companyPrefix, int reference);
    }
}
