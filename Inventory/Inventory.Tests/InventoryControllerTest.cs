using Inventory.API;
using Inventory.API.Models;
using Inventory.API.Models.DTOs;
using Inventory.API.Models.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Tests
{
    [TestClass]
    public class InventoryControllerTest
    {
        private static WebApplicationFactory<Startup> _factory;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                    builder.UseSetting("https_port", "44302").UseEnvironment("Testing")
                );
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory.Dispose();
        }

        //I suggest to run ShouldUploadProductDefinitions in ProductDefinitionControllerTest first
        //If no match for productdefinition the item won't be saved
        //[TestMethod]
        public async Task ShouldUploadInventoryItems()
        {
            using var client = _factory.CreateClient();

            string pathTags = Path.Combine(Environment.CurrentDirectory, @"Files\", "tags.txt");
            string[] linestags = System.IO.File.ReadAllLines(pathTags);

            var inventoryModel = new InventoryDTO()
            {
                InventoryId = "Inventory1",
                DateOfInventory = System.DateTime.Now.Date,
                InventoryLocation = "Austria",
                SGTIN96Tags = linestags.ToList()
            };

            var jsonToPost = JsonConvert.SerializeObject(inventoryModel);
            var data = new StringContent(jsonToPost, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/inventory/tags", data);

        }

        [TestMethod]
        public async Task ShouldCreateReadDelete()
        {
            //Create
            using var client = _factory.CreateClient();

            var productDefinition = new ProductDefinitionDTO()
            {
                CompanyPrefix = 614141,
                CompanyName = "TestCompany",
                ItemReference = 812345,
                ProductName = "TestProduct"
            };

            var jsonProduct = JsonConvert.SerializeObject(productDefinition);
            var dataProduct = new StringContent(jsonProduct, Encoding.UTF8, "application/json");
            var responseProduct = await client.PostAsync("api/product/productdefinition", dataProduct);

            Assert.IsTrue(responseProduct.StatusCode == System.Net.HttpStatusCode.OK ||
                responseProduct.Content.ReadAsStringAsync().Result.Contains("Product definition already exists!"));

            var inventoryModel = new InventoryDTO()
            {
                InventoryId = "InventoryCreateTest",
                DateOfInventory = DateTime.Now.Date,
                InventoryLocation = "Austria",
                SGTIN96Tags = new List<string>() { "3074257BF7194E4000001A85" }
            };

            var jsonInventory = JsonConvert.SerializeObject(inventoryModel);
            var inventoryData = new StringContent(jsonInventory, Encoding.UTF8, "application/json");
            var responseTags = await client.PostAsync("api/inventory/tags", inventoryData);

            Assert.IsTrue(responseTags.StatusCode == System.Net.HttpStatusCode.OK);

            //Read
            var responseRead = await client.GetAsync("api/inventory/items?inventoryId=InventoryCreateTest");
            var resultStream =  await responseRead.Content.ReadAsStreamAsync();
            List<ItemDetails> resultRead = null;
            using (var streamReader = new StreamReader(resultStream))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var jsonSerializer = new JsonSerializer();
                    resultRead = jsonSerializer.Deserialize<List<ItemDetails>>(jsonTextReader);
                }
            }

            Assert.IsTrue(responseRead.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.IsNotNull(resultRead);
            Assert.AreEqual(resultRead.Count,1);
            Assert.AreEqual(resultRead.First().InventoryId, "InventoryCreateTest");
            Assert.AreEqual(resultRead.First().CompanyPrefix, 614141);
            Assert.AreEqual(resultRead.First().ItemReference, 812345);
            Assert.AreEqual(resultRead.First().SerialNumber, 6789);

            //Delete
            var responseDelete = await client.DeleteAsync("api/inventory/Delete?inventoryId=InventoryCreateTest");
            Assert.IsTrue(responseDelete.StatusCode == System.Net.HttpStatusCode.OK);

            var responseDeleteProduct = await client.DeleteAsync("api/product/Delete?companyPrefix=614141&reference=812345");
            var jsonResponseDeleteProduct = await responseDeleteProduct.Content.ReadAsStringAsync();
            Assert.IsTrue(responseDeleteProduct.StatusCode == System.Net.HttpStatusCode.OK);


        }

        
    }
}
