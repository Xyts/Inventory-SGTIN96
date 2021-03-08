using Inventory.API.Models;
using Inventory.API.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory.Tests
{
    [TestClass]
    public class InventoryRepositoryTest
    {
        SqliteInventoryDbContext db;
        InventoryRepository inventoryrepository;
        ProductDefinitionRepository productRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            db = new SqliteInventoryDbContext();
            inventoryrepository = new InventoryRepository(db);
            productRepository = new ProductDefinitionRepository(db);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //Clean
            inventoryrepository.DeleteItemsByInventoryId("Inv1");
            inventoryrepository.DeleteItemsByInventoryId("Inv2");
            productRepository.DeleteProductByCompanyPrefixAndReference(1, 11);
            productRepository.DeleteProductByCompanyPrefixAndReference(1, 12);
            productRepository.DeleteProductByCompanyPrefixAndReference(2, 12);
            db.Dispose();
        }

        [TestMethod]
        public void ShouldGetQuantityByProductNumberForInventoryId()
        {
            //Arrange
            productRepository.InsertProductDefinition(
                    new ProductDefinitionModel()
                    {
                        CompanyPrefix = 1,
                        CompanyName = "Test1",
                        ItemReference = 11,
                        ProductName = "Test11",
                        Id = 1
                    });

            productRepository.InsertProductDefinition(
                    new ProductDefinitionModel()
                    {
                        CompanyPrefix = 1,
                        CompanyName = "Test1",
                        ItemReference = 12,
                        ProductName = "Test12",
                        Id = 2
                    });

            inventoryrepository.InsertInventoryInBulk(new List<InventoryModel>() {
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 1,
                        SerialNumber = 123456,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {  //Checking duplicates
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 1,
                        SerialNumber = 123456,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 2,
                        SerialNumber = 78916,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 2,
                        SerialNumber = 669887,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv2",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 2,
                        SerialNumber = 999999,
                        InventoryLocation = "Austria"
                    },

                });

            //Making sure no duplicates inserted
            inventoryrepository.InsertInventoryInBulk(new List<InventoryModel>() {
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 1,
                        SerialNumber = 123456,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 2,
                        SerialNumber = 78916,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 2,
                        SerialNumber = 669887,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv2",
                        DateOfInventory = DateTime.Now,
                        ProductDefinitionId = 2,
                        SerialNumber = 999999,
                        InventoryLocation = "Austria"
                    },

                });

            //Act
            var grouped = inventoryrepository.GetQuantityByProductNumberForInventoryId("Inv1");

            //Assert
            Assert.IsTrue(grouped.IsSuccess);
            Assert.IsNotNull(grouped.Value);
            Assert.AreEqual(grouped.Value.Count, 2);
            Assert.AreEqual(grouped.Value.FirstOrDefault(x => x.CompanyPrefix == 1 && x.ItemReference == 12).Count, 2);
            Assert.AreEqual(grouped.Value.FirstOrDefault(x => x.CompanyPrefix == 1 && x.ItemReference == 11).Count, 1);
        }

        [TestMethod]
        public void Should_GetQuantityByProductNumberAndDate()
        {
            //Arrange
            productRepository.InsertProductDefinition(
                    new ProductDefinitionModel()
                    {
                        CompanyPrefix = 1,
                        CompanyName = "Test1",
                        ItemReference = 11,
                        ProductName = "Test11",
                        Id = 1
                    });

            productRepository.InsertProductDefinition(
                    new ProductDefinitionModel()
                    {
                        CompanyPrefix = 1,
                        CompanyName = "Test1",
                        ItemReference = 12,
                        ProductName = "Test12",
                        Id = 2
                    });

            inventoryrepository.InsertInventoryInBulk(new List<InventoryModel>() {
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now.Date,
                        ProductDefinitionId = 1,
                        SerialNumber = 123456,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now.Date,
                        ProductDefinitionId = 2,
                        SerialNumber = 78916,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv1",
                        DateOfInventory = DateTime.Now.Date.AddDays(1),
                        ProductDefinitionId = 2,
                        SerialNumber = 669887,
                        InventoryLocation = "Austria"
                    },
                    new InventoryModel()
                    {
                        InventoryId = "Inv2",
                        DateOfInventory = DateTime.Now.Date.AddDays(1),
                        ProductDefinitionId = 2,
                        SerialNumber = 999999,
                        InventoryLocation = "Austria"
                    }
                });

            //Act
            var grouped = inventoryrepository.GetQuantityByProductNumberAndDate();

            //Assert
            Assert.IsTrue(grouped.IsSuccess);
            Assert.IsNotNull(grouped.Value);
            Assert.AreEqual(grouped.Value.Count, 3);
            Assert.AreEqual(
                grouped.Value.FirstOrDefault(x => x.DateOfInventory == DateTime.Now.Date.AddDays(1) && x.CompanyPrefix == 1 && x.ItemReference == 12).Count, 2);
            Assert.AreEqual(grouped.Value.FirstOrDefault(x => x.DateOfInventory == DateTime.Now.Date && x.CompanyPrefix == 1 && x.ItemReference == 11).Count, 1);
            Assert.AreEqual(grouped.Value.FirstOrDefault(x => x.DateOfInventory == DateTime.Now.Date && x.CompanyPrefix == 1 && x.ItemReference == 12).Count, 1);
           
        }

        [TestMethod]
        public void Should_GetQuantityByCompany()
        {
            //Arrange
            productRepository.InsertProductDefinition(
               new ProductDefinitionModel()
               {
                   CompanyPrefix = 1,
                   CompanyName = "Test1",
                   ItemReference = 11,
                   ProductName = "Test11",
                   Id = 1
               });
            productRepository.InsertProductDefinition(new ProductDefinitionModel()
                {
                    CompanyPrefix = 2,
                    CompanyName = "Test1",
                    ItemReference = 12,
                    ProductName = "Test12",
                    Id = 2
                });

            inventoryrepository.InsertInventoryInBulk(new List<InventoryModel>() {
                new InventoryModel()
                {
                    InventoryId = "Inv1",
                    DateOfInventory = DateTime.Now.Date,
                    ProductDefinitionId = 1,
                    SerialNumber = 123456,
                    InventoryLocation = "Austria"
                },
                new InventoryModel()
                {
                    InventoryId = "Inv1",
                    DateOfInventory = DateTime.Now.Date,
                    ProductDefinitionId = 2,
                    SerialNumber = 78916,
                    InventoryLocation = "Austria"
                },
                new InventoryModel()
                {
                    InventoryId = "Inv1",
                    DateOfInventory = DateTime.Now.Date.AddDays(1),
                    ProductDefinitionId = 2,
                    SerialNumber = 669887,
                    InventoryLocation = "Austria"
                },
                new InventoryModel()
                {
                    InventoryId = "Inv2",
                    DateOfInventory = DateTime.Now.Date.AddDays(1),
                    ProductDefinitionId = 2,
                    SerialNumber = 999999,
                    InventoryLocation = "Austria"
                }
            });

            //Act
            var grouped = inventoryrepository.GetQuantityByCompany();

            //Assert
            Assert.IsTrue(grouped.IsSuccess);
            Assert.IsNotNull(grouped.Value);
            Assert.AreEqual(grouped.Value.Count, 2);
            Assert.AreEqual(
                grouped.Value.FirstOrDefault(x => x.CompanyPrefix == 1).Count, 1);
            Assert.AreEqual(
                grouped.Value.FirstOrDefault(x => x.CompanyPrefix == 2).Count, 3);

            
        }
        
    }
}
