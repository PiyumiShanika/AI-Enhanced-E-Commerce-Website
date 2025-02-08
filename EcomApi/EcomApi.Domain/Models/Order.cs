using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class Order
    {
        public int Order_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public Double price { get; set; }
        public String Order_Status { get; set; }
        public string Payment_ID { get; set; }
        public string Strip_session_Id { get; set; }


        //point out the user model
        public User User { get; set; }
        public string User_Id { get; set; }

        //orderProduct 
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
