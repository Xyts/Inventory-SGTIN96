using Inventory.API.Models;
using Inventory.API.Models.Queries;
using Inventory.API.Models.Query;
using System;
using System.Collections.Generic;

namespace Inventory.API.Services.Interfaces
{
    public interface IInventoryService
    {
        Result SaveInventory(string inventoryId, string inventoryLocation, DateTime dateOfInventory, List<string> SGTIN96Tags);
        Result<List<QuantityByProductNumber>> GetQuantityByProductNumberForInventoryId(string inventoryId);
        Result<List<QuantityByProductNumberAndDate>> GetQuantityByProductNumberAndDate();
        Result<List<QuantityByCompany>> GetQuantityByCompany();
        Result<List<ItemDetails>> GetItemsByInventoryId(string inventoryId);
        void DeleteItemsByInventoryId(string inventoryId);
    }
}
