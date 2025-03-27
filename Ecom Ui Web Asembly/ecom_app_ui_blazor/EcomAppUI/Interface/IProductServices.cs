using EcomAppUI.Models;

namespace EcomAppUI.Interface
{
    public interface IProductServices
    {
        Task<List<ProductModel>> GetAllProducts();
        Task<List<CategoryModel>> GetAllCategories();
        Task<List<ProductModel>> GetProductsByCategory(int categoryId);
    }
}
