using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class User
    {

        public string User_Id { get; set; }
        public string first_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string? Strip_Cust_Id { get; set; }

        public virtual ICollection<Order> orders { get; set; }
        public virtual ICollection<Address> addresses { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }
    }
}
