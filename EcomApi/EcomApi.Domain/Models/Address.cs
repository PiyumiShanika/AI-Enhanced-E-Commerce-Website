using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class Address
    {
        public int Adress_Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int Postal_Code { get; set; }
        public bool isPrimary { get; set; }

        //point out the user model
        public User User { get; set; }
        public string User_Id { get; set; }


    }
}
