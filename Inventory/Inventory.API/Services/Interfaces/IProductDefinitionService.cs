using Inventory.API.Models;

namespace Inventory.API.Services.Interfaces
{
    public interface IProductDefinitionService
    {
        Result SaveProductDefinition(long companyPrefix, string companyName, int itemReference, string productName);
        void DeleteProductByCompanyPrefixAndReference(long companyPrefix, int reference);
    }
}
