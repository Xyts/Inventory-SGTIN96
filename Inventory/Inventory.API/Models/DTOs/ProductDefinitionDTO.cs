namespace Inventory.API.Models.DTOs
{
    public class ProductDefinitionDTO
    {
        public long CompanyPrefix { get; set; }
        public string CompanyName { get; set; }
        public int ItemReference { get; set; }
        public string ProductName { get; set; }
    }
}
