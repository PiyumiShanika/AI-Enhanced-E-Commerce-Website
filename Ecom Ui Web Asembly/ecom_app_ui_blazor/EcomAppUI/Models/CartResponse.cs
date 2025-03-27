namespace EcomAppUI.Models
{
    public class CartResponse
    {
        public List<CartProduct> Cart { get; set; }
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime CartCreatedAt { get; set; }
        public DateTime CartUpdatedAt { get; set; }
    }
}
