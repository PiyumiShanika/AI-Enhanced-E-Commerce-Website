using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class UserProduct
    {
        public User user { get; set; }
        public string User_ID { get; set; }
        public product product { get; set; }
        public int Product_ID { get; set; }
        public int Quntity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //overall cart 
        public DateTime CartCreatedAt { get; set; }
        public DateTime? cartUpdatedAt { get; set; }
    }
}
