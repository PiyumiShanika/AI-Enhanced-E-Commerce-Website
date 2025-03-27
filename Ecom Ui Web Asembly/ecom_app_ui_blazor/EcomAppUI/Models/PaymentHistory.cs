namespace EcomAppUI.Models
{
    public class PaymentHistory
    {
        public int Order_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public Double price { get; set; }
        public String Order_Status { get; set; }
        public string Payment_ID { get; set; }
    }
}