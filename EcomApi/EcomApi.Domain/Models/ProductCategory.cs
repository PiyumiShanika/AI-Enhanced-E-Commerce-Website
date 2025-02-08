using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.Models
{
    public class ProductCategory
    {
        public int Category_Id { get; set; }
        public string Name { get; set; }
        public string Image_Url { get; set; }

        public virtual ICollection<product> products { get; set; }

    }
}
