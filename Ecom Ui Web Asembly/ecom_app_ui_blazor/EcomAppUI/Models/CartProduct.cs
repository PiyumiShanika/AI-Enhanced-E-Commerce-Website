namespace EcomAppUI.Models
{
    public class CartProduct
    {
        public int Product_Id { get; set; }
        public string Name { get; set; }
        public string Image_Url { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public int Quntity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int Category_Id { get; set; }
        //to track the item count
        public int ItemCount { get; set; } = 1;
    }
}
