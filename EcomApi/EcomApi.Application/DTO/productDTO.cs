using EcomApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class productDTO
    {
        public int Product_Id { get; set; }
        public string Name { get; set; }
        public string Image_Url { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public int Quntity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int Category_Id { get; set; }
    }
}
