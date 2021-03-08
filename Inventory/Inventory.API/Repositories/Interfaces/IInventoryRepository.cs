using Inventory.API.Models;
using Inventory.API.Models.Queries;
using Inventory.API.Models.Query;
using System.Collections.Generic;

namespace Inventory.API.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        void InsertInventory(InventoryModel inventoryModel);
        void InsertInventoryInBulk(IEnumerable<InventoryModel> inventoryModel);
        InventoryModel GetItemByDefinitionIdAndSerial(long productDefinitionId, long serialNumber);
        Result<List<QuantityByProductNumber>> GetQuantityByProductNumberForInventoryId(string inventoryId);
        Result<List<QuantityByProductNumberAndDate>> GetQuantityByProductNumberAndDate();
        Result<List<QuantityByCompany>> GetQuantityByCompany();
        Result<List<ItemDetails>> GetItemsByInventoryId(string inventoryId);
        void DeleteItemsByInventoryId(string inventoryId);
    }
}
