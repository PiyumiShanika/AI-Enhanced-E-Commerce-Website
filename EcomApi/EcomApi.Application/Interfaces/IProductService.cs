using EcomApi.Domain.Models;

namespace EcomApi.Application.Interfaces
{
    public interface IProductService
    {
        //product category add
        // Task<ProductCategory> CreateProductCategory(ProductCategoryDTO productCategoryDTOReq);

        //product add
        //  Task<product> creteProduct(productDTO productDTOReq);

        //get products
        Task<List<product>> GetProducts();

        //get product by category
        Task<List<product>> getProductCategory(int proId);

        //get product by id
        Task<product> getProductById(int productID);

        //category vise all product
        Task<List<ProductCategory>> GetProductswithCategory();
    }
}
