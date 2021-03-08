using Inventory.API;
using Inventory.API.Models;
using Inventory.API.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Tests
{
    [TestClass]
    public class ProductDefinitionControllerTest
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

        [TestMethod]
        public async Task ShouldUploadProductDefinitions()
        {
            using var client = _factory.CreateClient();

            string pathDefinition = Path.Combine(Environment.CurrentDirectory, @"Files\", "data.csv");

            string[] linesDefinitions = System.IO.File.ReadAllLines(pathDefinition);

            for (int i = 1; i < linesDefinitions.Count(); i++)
            {
                string[] columns = linesDefinitions[i].Split(';');
                var productDefinition = new ProductDefinitionDTO(){
                    CompanyPrefix = Convert.ToInt64(columns[0]),
                    CompanyName = columns[1],
                    ItemReference = Convert.ToInt32(columns[2]),
                     ProductName = columns[3]
                };

                var jsonToPost = JsonConvert.SerializeObject(productDefinition);
                var data = new StringContent(jsonToPost, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/product/productdefinition", data);
            }
            
        }
    }
}
