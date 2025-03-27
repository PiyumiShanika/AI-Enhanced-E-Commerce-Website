using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class PaymentHistoryItemDTO
    {
        public string Image_Url { get; set; }
        public string Product_Name { get; set; }
        public int Quantity { get; set; }
        public double Unite_Price { get; set; }
        public double Sub_Total { get; set; }
    }
}
