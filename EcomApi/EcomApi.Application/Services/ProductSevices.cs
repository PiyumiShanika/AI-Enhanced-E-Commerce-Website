using EcomApi.Domain.Models;
using Serilog;
using EcomApi.Application.Interfaces;

namespace EcomApi.Application.Serrvices
{
    public class ProductServices : IProductService
    {
        private readonly ITransaction _transaction;

        public ProductServices(ITransaction transaction)
        {

            _transaction = transaction;
        }
        //product category add method
        //public async Task<ProductCategory> CreateProductCategory(ProductCategoryDTO productCategoryDTOReq)
        //{
        //    var myEntity = new ProductCategory
        //    {
        //        Category_Id = productCategoryDTOReq.Category_Id,
        //        Name = productCategoryDTOReq.Name, 
        //        Image_Url = productCategoryDTOReq.Image_Url,
        //    };

        //    // Add the entity to the context
        //    _appDbContext.productsCategories.Add(myEntity);

        //    try
        //    {
        //        await _appDbContext.SaveChangesAsync();
        //        return myEntity;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error creating entity", ex);
        //    }
        //}

        //add product method
        //public async Task<product> creteProduct(productDTO productDTOReq)
        //{
        //    var myEntity = new product
        //    {
        //       Product_Id = productDTOReq.Product_Id,
        //       Name= productDTOReq.Name,
        //       Image_Url= productDTOReq.Image_Url,
        //       Price = productDTOReq.Price,
        //       Discount = productDTOReq.Discount,
        //       Description = productDTOReq.Description,
        //       Category_Id=productDTOReq.Category_Id,
        //       Stock=productDTOReq.Stock,
        //    };

        //    // Add the entity to the context
        //    _appDbContext.products.Add(myEntity);

        //    try
        //    {
        //        await _appDbContext.SaveChangesAsync();
        //        return myEntity;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("Error creating entity", ex);
        //    }
        //}

        //view produt List
        public async Task<List<product>> GetProducts()
        {
            try
            {

                var products = await _transaction.GetRepository<product>().GetAllAsync();
                if (products == null)
                {
                    Log.Warning("No products found.");
                }
                else
                {
                    Log.Information("Successfully fetched {ProductCount} products: {ProductDetails}", products.Count, string.Join(", ", products.Select(p => p.Name)));
                }
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error fetching products. Please try again later.");
            }
        }


        //get product acording to the category to front end
        public async Task<List<ProductCategory>> GetProductswithCategory()
        {
            try
            {
                var categories = await _transaction.GetRepository<ProductCategory>().GetAllAsync();
                if (categories == null)
                {
                    Log.Warning("No products category found.");
                }
                else
                {

                    Log.Information("Successfully fetched {CategoryCount} product categories: {CategoryDetails}", categories.Count, string.Join(", ", categories.Select(c => c.Name)));
                }
                return categories;
            }
            catch (Exception ex)
            {

                throw new Exception("There was an error fetching products. Please try again later.");
            }
        }
        //get product by category
        public async Task<List<product>> getProductCategory(int categoryID)
        {
            try
            {

                var productCategory = await _transaction.GetRepository<product>().GetByIdAsync(categoryID, a => a.Category_Id == categoryID);
                if (productCategory != null)
                {
                    Log.Information("Successfully fetched {ProductCount} products for category with ID: {CategoryID}.", productCategory.Count, categoryID);
                }
                else
                {
                    Log.Warning("No products found for category with ID: {CategoryID}.", categoryID);
                }
                return productCategory;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching products for category with ID: {CategoryID}.", categoryID);
                throw new Exception("There was an error fetching products for the specified category. Please try again later.");
            }
        }

        //get product by id
        public async Task<product> getProductById(int productID)
        {
            try
            {
                var product = await _transaction.GetRepository<product>().GetById(productID);

                if (product != null)
                {
                    Log.Information("Successfully fetched product: {ProductId} - {ProductName}", product.Product_Id, product.Name);
                }
                else
                {
                    Log.Warning("Product with ID: {ProductId} not found.", productID);
                }
                return product;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching product with ID: {ProductId}.", productID);
                throw new Exception("There was an error fetching the product. Please try again later.");
            }
        }

    }
}
