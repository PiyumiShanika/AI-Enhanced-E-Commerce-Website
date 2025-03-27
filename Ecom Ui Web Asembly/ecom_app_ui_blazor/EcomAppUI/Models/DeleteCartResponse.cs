namespace EcomAppUI.Models
{
    public class DeleteCartResponse
    {
        
        public string User_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quntity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //overall cart 
        public DateTime CartCreatedAt { get; set; }
        public DateTime? cartUpdatedAt { get; set; }
    }
}
