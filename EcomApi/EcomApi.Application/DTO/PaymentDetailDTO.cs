using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class PaymentDetailDTO
    {
        public string Description { get; set; }
        public long? Quantity { get; set; }
        public long? UnitPrice { get; set; }
        public long? Amount { get; set; }
    }
}
