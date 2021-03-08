using Inventory.API.Models;
using Inventory.API.Models.Queries;
using Inventory.API.Models.Query;
using Inventory.API.Repositories.Interfaces;
using Inventory.API.Services.Interfaces;
using Inventory.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Inventory.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly IProductDefinitionRepository productDefinitionRepository;

        public InventoryService(IInventoryRepository inventoryRepository, IProductDefinitionRepository productDefinitionRepository)
        {
            this.inventoryRepository = inventoryRepository;
            this.productDefinitionRepository = productDefinitionRepository;
        }

        public Result SaveInventory(string inventoryId, string inventoryLocation, DateTime dateOfInventory, List<string> SGTIN96Tags)
        {
            if (!Regex.IsMatch(inventoryId, "^[a-zA-Z0-9]+$")) //Alphanumeric, no empty string
            {
                return Result.Fail("Inventory Id has to be alphanumeric.");
            }
            var inventories = new List<InventoryModel>();

            foreach (var tag in SGTIN96Tags)
            {
                var sgtin96Data = ProcessSGTIN96(tag);
                if (sgtin96Data.IsFailure)
                {
                    //log error here
                    continue;
                }

                //Todo: Performance tip: Some sort of caching mechanism might be usefule here 
                //to avoid going to the database in every iteration
                var productdefinition = productDefinitionRepository.GetProductdefinitionByCompanyPrefixAndReference(sgtin96Data.Value.CompanyPrefix, sgtin96Data.Value.ItemReference);
                if (productdefinition == null)
                {
                    //log error here
                    continue;
                }

                var inventory = new InventoryModel()
                {
                    InventoryId = inventoryId,
                    InventoryLocation = inventoryLocation,
                    DateOfInventory = dateOfInventory,
                    SerialNumber = sgtin96Data.Value.SerialNumber,
                    ProductDefinitionId = productdefinition.Id
                };

                inventories.Add(inventory);
            }

            if (!inventories.Any())
            {
                return Result.Fail("No items were added to the inventory. Please check the format of the tags or the product definitions.");
            }

            inventoryRepository.InsertInventoryInBulk(inventories);
            return Result.Ok();
        }

        public Result<SGTIN96Data> ProcessSGTIN96(string tag)
        {

            if (!Regex.IsMatch(tag, "^[a-fA-F0-9]{24}$")) //Hex, 24 characters long
            {
                return Result.Fail<SGTIN96Data>("Incorrect tag format!");
            }

            var tagInBinary = tag.HexToBinary();

            var header = tagInBinary.Substring(0, 8);
            if (header != "00110000")
            {
                return Result.Fail<SGTIN96Data>("Incorrect header!");
            }

            var partion = Convert.ToInt32(tagInBinary.Substring(11, 3), 2);
            int companyPrefixNumberOfBits;
            int referenceNumberOfBits;
            switch (partion)
            {
                case 0:
                    companyPrefixNumberOfBits = 40;
                    referenceNumberOfBits = 4;
                    break;
                case 1:
                    companyPrefixNumberOfBits = 37;
                    referenceNumberOfBits = 7;
                    break;
                case 2:
                    companyPrefixNumberOfBits = 34;
                    referenceNumberOfBits = 10;
                    break;
                case 3:
                    companyPrefixNumberOfBits = 30;
                    referenceNumberOfBits = 14;
                    break;
                case 4:
                    companyPrefixNumberOfBits = 27;
                    referenceNumberOfBits = 17;
                    break;
                case 5:
                    companyPrefixNumberOfBits = 24;
                    referenceNumberOfBits = 20;
                    break;
                case 6:
                    companyPrefixNumberOfBits = 20;
                    referenceNumberOfBits = 24;
                    break;
                default:
                    return Result.Fail<SGTIN96Data>("Wrong partion!");
            }

            var companyPrefixInBinary = tagInBinary.Substring(14, companyPrefixNumberOfBits);
            var itemReferenceInBinary = tagInBinary.Substring(14 + companyPrefixNumberOfBits, referenceNumberOfBits);
            var serialInBinary = tagInBinary.Substring(14 + companyPrefixNumberOfBits + referenceNumberOfBits);

            var companyPrefix = Convert.ToInt64(companyPrefixInBinary, 2); //max 12 digit
            var itemReference = Convert.ToInt32(itemReferenceInBinary, 2); //max 7 digit
            var serialNumber = Convert.ToInt64(serialInBinary, 2); //max 12 digit

            return Result.Ok(new SGTIN96Data { CompanyPrefix = companyPrefix, ItemReference = itemReference, SerialNumber = serialNumber });
        }

        public Result<List<QuantityByProductNumber>> GetQuantityByProductNumberForInventoryId(string inventoryId)
        {
            return inventoryRepository.GetQuantityByProductNumberForInventoryId(inventoryId);
        }

        public Result<List<QuantityByProductNumberAndDate>> GetQuantityByProductNumberAndDate()
        {
            return inventoryRepository.GetQuantityByProductNumberAndDate();
        }

        public Result<List<QuantityByCompany>> GetQuantityByCompany()
        {
            return inventoryRepository.GetQuantityByCompany();
        }

        public Result<List<ItemDetails>> GetItemsByInventoryId(string inventoryId)
        {
            return inventoryRepository.GetItemsByInventoryId(inventoryId);
        }

        public void DeleteItemsByInventoryId(string inventoryId)
        {
            inventoryRepository.DeleteItemsByInventoryId(inventoryId);
        }
    }
}
