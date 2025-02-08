using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class OrderProduct
    {
        //order table point
        public Order order { get; set; }
        public int Order_Id { get; set; }

        //product table point
        public product product { get; set; }
        public int Product_Id { get; set; }

        public int Order_Qty { get; set; }
        public double Unite_Price { get; set; }
    }
}
