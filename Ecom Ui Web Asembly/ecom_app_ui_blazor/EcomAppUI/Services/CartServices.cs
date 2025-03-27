using Blazored.SessionStorage;
using EcomAppUI.Interface;
using EcomAppUI.Models;
using EcomAppUI.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EcomAppUI.Services
{
    public class CartServices : ICartServices
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorage;
        private readonly IJSRuntime _jsruntime;
        private int totalItems;
        private decimal subTotal;

    

        public CartServices(HttpClient httpClient , ISessionStorageService sessionStorageService , IJSRuntime jSRuntime)
        { 
            _httpClient = httpClient;
            _sessionStorage = sessionStorageService;
            _jsruntime = jSRuntime; 
        }

        public async Task<string> AddToCart(UserCart userCart)
        {
            try 
            {
                var token = await _sessionStorage.GetItemAsync<string>("access_Token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var result = await _httpClient.PostAsJsonAsync<UserCart>("https://localhost:7075/api/cart/AddTo-Cart", userCart);
                if (result.IsSuccessStatusCode)
                {
                    Console.WriteLine("Item Add to Cart Done in FrontEnd");
                  
                    return "200";
                   
                }
                else
                {
                    return result.ToString();
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
        
        public async Task<CartResponse> ViewCart()
        {
            var token = await _sessionStorage.GetItemAsync<string>("access_Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var cartResponse = await _httpClient.GetFromJsonAsync<CartResponse>("https://localhost:7075/api/cart/View-Cart");
           
            return cartResponse;
           
        }

      
        public async Task<DeleteCartResponse> DeleteCartItem(int productId)
        {
            try
            {
                var token = await _sessionStorage.GetItemAsync<string>("access_Token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"https://localhost:7075/api/cart/Delete-cart-Item?productid={productId}");
                response.EnsureSuccessStatusCode();
               
                return await response.Content.ReadFromJsonAsync<DeleteCartResponse>();
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Error deleting item {productId}: {ex.Message}");
            }
        }

        //update cart item
        public async Task<CartResponse> UpdateCartItemQuantityAsync(int productId, int newQuantity)
        {
            var token = await _sessionStorage.GetItemAsync<string>("access_Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"https://localhost:7075/api/cart/update-item-quantity?productId={productId}&newQuantity={newQuantity}";
            var response = await _httpClient.PatchAsync(url, null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Cart item Update Sucessfully for item {productId} and new quntity is {newQuantity}");
                return await response.Content.ReadFromJsonAsync<CartResponse>();
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error updating cart item quantity: {errorMessage}");
            }
        }

        //purchase cart 
        public async Task<string> CreateCheckoutSession()
        {
            try
            {
                var token = await _sessionStorage.GetItemAsync<string>("access_Token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync("https://localhost:7075/api/payment/purchase-cart", null);
                response.EnsureSuccessStatusCode(); // Ensure HTTP 200-299

                return await response.Content.ReadAsStringAsync();
               
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new Exception("An error occurred while creating the checkout session.", ex);
            }
        }
    }
}
    

