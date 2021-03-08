using Inventory.API.Models;
using Inventory.API.Models.Queries;
using Inventory.API.Models.Query;
using Inventory.API.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.API.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        InventoryDbContext inventoryDbContext;

        public InventoryRepository(InventoryDbContext inventoryDbContext)
        {
            this.inventoryDbContext = inventoryDbContext;
        }

        public void InsertInventory(InventoryModel inventoryModel)
        {
            inventoryDbContext.Inventories.Add(inventoryModel);
            inventoryDbContext.SaveChanges();
        }

        public InventoryModel GetItemByDefinitionIdAndSerial(long productDefinitionId, long serialNumber)
        {
            var query = from inventory in inventoryDbContext.Inventories
                        where inventory.ProductDefinitionId == productDefinitionId && inventory.SerialNumber == serialNumber
                        select inventory;

            return query.FirstOrDefault();
        }

        public Result<List<ItemDetails>> GetItemsByInventoryId(string inventoryId)
        {
            var query = from inventory in inventoryDbContext.Inventories
                        join product in inventoryDbContext.ProductDefinitions on inventory.ProductDefinitionId equals product.Id
                        where inventory.InventoryId == inventoryId
                        select new ItemDetails()
                        {
                            InventoryId = inventory.InventoryId,
                            InventoryLocation = inventory.InventoryLocation,
                            CompanyPrefix = product.CompanyPrefix,
                            CompanyName = product.CompanyName,
                            ProductName = product.ProductName,
                            ItemReference = product.ItemReference,
                            SerialNumber = inventory.SerialNumber,
                            DateOfInventory = inventory.DateOfInventory
                        };

            return Result.Ok(query.ToList());
        }

        public void DeleteItemsByInventoryId(string inventoryId)
        {
            var itemsToRemove = inventoryDbContext.Inventories.Where( x => x.InventoryId == inventoryId);
            if (itemsToRemove.Any())
            {
                inventoryDbContext.Inventories.RemoveRange(itemsToRemove);
            }
            
            inventoryDbContext.SaveChanges();
        }

        public void InsertInventoryInBulk(IEnumerable<InventoryModel> inventoryModels)
        {
            //remove duplicates
            inventoryModels = inventoryModels.Distinct(new InventoryComparer());

            //Todo: Options here: Don't do anything with duplicates (current solution)
            // Relocate the duplicates to the new inventory location
            // FYI: Unique index on the table pereventing duplicates 
            inventoryDbContext.Inventories.BulkInsert(inventoryModels, options =>
            {
                options.ColumnPrimaryKeyExpression = x => new { x.ProductDefinitionId, x.SerialNumber };
                options.InsertIfNotExists = true;
            });
        }

        public Result<List<QuantityByProductNumber>> GetQuantityByProductNumberForInventoryId(string inventoryId)
        {
            //we need both CompanyPrefix and ItemReference here becuse they both needed to uniquely indentify a product. Different compenies might have same ItemReference number
            var query = from inventory in inventoryDbContext.Inventories
                        join product in inventoryDbContext.ProductDefinitions on inventory.ProductDefinitionId equals product.Id
                        where inventory.InventoryId == inventoryId
                        group new { inventory, product } by new { inventory.InventoryId, product.CompanyPrefix, product.ItemReference } into gcs
                        select new QuantityByProductNumber
                        {
                            CompanyPrefix = gcs.Key.CompanyPrefix,
                            ItemReference = gcs.Key.ItemReference,
                            Count = gcs.Count()
                        };

            return Result.Ok(query.ToList());
        }

        public Result<List<QuantityByProductNumberAndDate>> GetQuantityByProductNumberAndDate()
        {
            //we need both CompanyPrefix and ItemReference here becuse they both needed to uniquely indentify a product. Different compenies might have same ItemReference number
            var query = from inventory in inventoryDbContext.Inventories
                        join product in inventoryDbContext.ProductDefinitions on inventory.ProductDefinitionId equals product.Id
                        group new { inventory, product } by new { inventory.DateOfInventory.Date, product.CompanyPrefix, product.ItemReference } into gcs
                        select new QuantityByProductNumberAndDate
                        {
                            DateOfInventory = gcs.Key.Date,
                            CompanyPrefix = gcs.Key.CompanyPrefix,
                            ItemReference = gcs.Key.ItemReference,
                            Count = gcs.Count()
                        };

            return Result.Ok(query.ToList());
        }

        public Result<List<QuantityByCompany>> GetQuantityByCompany()
        {
            var query = from inventory in inventoryDbContext.Inventories
                        join product in inventoryDbContext.ProductDefinitions on inventory.ProductDefinitionId equals product.Id
                        group product by new { product.CompanyPrefix } into gcs
                        select new QuantityByCompany
                        {
                            CompanyPrefix = gcs.Key.CompanyPrefix,
                            Count = gcs.Count()
                        };

            return Result.Ok(query.ToList());
        }
    }

    // Custom comparer for the InventoryModel class
    class InventoryComparer : IEqualityComparer<InventoryModel>
    {
        public bool Equals(InventoryModel x, InventoryModel y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ProductDefinitionId == y.ProductDefinitionId && x.SerialNumber == y.SerialNumber;
        }

        // If Equals() returns true for a pair of objects
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(InventoryModel inventory)
        {
            if (Object.ReferenceEquals(inventory, null)) return 0;

            int hashProductDefinitionId = inventory.ProductDefinitionId.GetHashCode();

            int hashSerialNumber = inventory.SerialNumber.GetHashCode();

            return hashProductDefinitionId ^ hashSerialNumber;
        }
    }
}
