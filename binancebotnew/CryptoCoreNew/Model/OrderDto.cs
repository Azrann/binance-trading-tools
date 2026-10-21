namespace CryptoCoreNew.Model
{
    public class OrderDto
    {
        public string OrderId { get; set; }
        public decimal Quantity { get; set; }
        public string Side { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }
}
