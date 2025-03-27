using EcomAppUI.Interface;
using EcomAppUI.Models;
using System.Net.Http.Json;


namespace EcomAppUI.Services
{
    public class ProductServices : IProductServices
    {
       // private readonly HttpClient _httpClient;
       private readonly HttpClient _httpClient;

        public ProductServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        //get all products as list
        public async Task<List<ProductModel>> GetAllProducts()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductModel>>("https://localhost:7075/api/product/Products");
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        //product categories
        public async Task<List<CategoryModel>> GetAllCategories()
        {
            var result = await _httpClient.GetFromJsonAsync<List<CategoryModel>>("https://localhost:7075/api/product/Category");
            return result;
        }

        public async Task<List<ProductModel>> GetProductsByCategory(int categoryId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<ProductModel>>($"https://localhost:7075/api/product/Product-Category/{categoryId}");
            return result;
        }
    }
}
