using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class PaymentHistoryDTO
    {
        public int Order_Id { get; set; }
        public DateTime Order_Date { get; set; }
        public Double price { get; set; }
        public String Order_Status { get; set; }
        public string Payment_ID { get; set; }

    }
}
