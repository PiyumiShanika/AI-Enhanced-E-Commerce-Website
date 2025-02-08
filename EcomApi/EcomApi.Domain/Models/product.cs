using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class product
    {
        public int Product_Id { get; set; }
        public string Name { get; set; }
        public string Image_Url { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }

        //orderProduct 
        public ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }

        //point the product cate table
        public ProductCategory productCategory { get; set; }
        public int Category_Id { get; set; }
    }
}
