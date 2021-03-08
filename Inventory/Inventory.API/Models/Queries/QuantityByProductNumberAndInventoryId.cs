namespace Inventory.API.Models.Query
{
    public class QuantityByProductNumber
    {
        public long CompanyPrefix { get; set; }
        public int ItemReference { get; set; }
        public int Count { get; set; }
    }
}
