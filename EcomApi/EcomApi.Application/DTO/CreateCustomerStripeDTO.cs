using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class CreateCustomerStripeDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
