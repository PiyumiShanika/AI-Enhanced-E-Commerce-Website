using EcomAppUI.Interface;
using EcomAppUI.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using EcomAppUI.Pages;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace EcomAppUI.Services
{
    public class OrderServices : IOrderServics
    {

        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorage;
        private readonly IJSRuntime _jsruntime;




        public OrderServices(HttpClient httpClient, ISessionStorageService sessionStorageService, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorageService;
            _jsruntime = jSRuntime;
        }


        public async Task<List<PaymentHistory>> ViewOrders()
        {
            var token = await _sessionStorage.GetItemAsync<string>("access_Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var orderResponse = await _httpClient.GetFromJsonAsync<List<PaymentHistory>>("https://localhost:7075/api/payment/payment-history");
            return orderResponse;
        }
        public async Task<List<PaymentHistroyItem>> ViewOrderitems(int order_Id)
        {
            var token = await _sessionStorage.GetItemAsync<string>("access_Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var orderResponse = await _httpClient.GetFromJsonAsync<List<PaymentHistroyItem>>($"https://localhost:7075/api/payment/payment-item-history/{order_Id}");
            return orderResponse;

        }
    }
}
