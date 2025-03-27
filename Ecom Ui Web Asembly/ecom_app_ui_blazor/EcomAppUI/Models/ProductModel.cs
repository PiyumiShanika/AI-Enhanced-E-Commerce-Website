namespace EcomAppUI.Models
{
    public class ProductModel
    {
        public int Product_Id { get; set; }

        public string Name { get; set; }

        public string Image_Url { get; set; }

        public double Price { get; set; }

        public double Discount { get; set; }

        public string Description { get; set; }

        public int Stock { get; set; }
        public int Category_Id { get; set; }
    }
}
