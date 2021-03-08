using Inventory.API.Models;
using Inventory.API.Repositories.Interfaces;
using Inventory.API.Services.Interfaces;

namespace Inventory.API.Services
{
    public class ProductDefinitionService : IProductDefinitionService
    {
        private readonly IProductDefinitionRepository productDefinitionRepository;
        public ProductDefinitionService(IProductDefinitionRepository productDefinitionRepository)
        {
            this.productDefinitionRepository = productDefinitionRepository;
        }
        public Result SaveProductDefinition(long companyPrefix, string companyName, int itemReference, string productName)
        {
            var validation = ValidateProductDefinition(companyPrefix, companyName, itemReference, productName);
            if (validation.IsFailure)
            {
                return Result.Fail(validation.Error);
            }

            var existingDefinition = productDefinitionRepository.GetProductdefinitionByCompanyPrefixAndReference(companyPrefix, itemReference);
            if (existingDefinition != null)
            {
                return Result.Fail("Product definition already exists!");
            }

            productDefinitionRepository.InsertProductDefinition(new ProductDefinitionModel()
            {
                CompanyPrefix = companyPrefix,
                CompanyName = companyName,
                ItemReference = itemReference,
                ProductName = productName
            });

            return Result.Ok();
        }

        private Result ValidateProductDefinition(long companyPrefix, string companyName, int itemReference, string productName)
        {
            if (companyPrefix == 0)
            {
                return Result.Fail("Company prefix cannot be 0!");
            }

            if (string.IsNullOrWhiteSpace(companyName))
            {
                return Result.Fail("Company name cannot be blank!");
            }

            if (itemReference == 0)
            {
                return Result.Fail("Item reference cannot be 0!");
            }

            if (string.IsNullOrWhiteSpace(productName))
            {
                return Result.Fail("Product name cannot be blank!");
            }

            return Result.Ok();
        }

        public void DeleteProductByCompanyPrefixAndReference(long companyPrefix, int reference)
        {
            productDefinitionRepository.DeleteProductByCompanyPrefixAndReference(companyPrefix,reference);
        }
    }
}
