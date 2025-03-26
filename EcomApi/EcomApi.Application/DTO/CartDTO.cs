using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class CartDTO
    {
        public int Product_ID { get; set; }
        public int Quntity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
