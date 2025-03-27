using EcomAppUI.Interface;
using EcomAppUI.Models;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace EcomAppUI.Services
{//     var response = await _httpClient.PostAsJsonAsync("https://localhost:7075/api/user/login", loginData);
    public class UserServices : IUserServices
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jSRuntime;

        public UserServices(HttpClient httpClient, IJSRuntime jSRuntime)
        {
            _httpClient = httpClient;
            _jSRuntime = jSRuntime;
        }

        // User signin service
        public async Task<Auth0TokenResponse> GetAccessToken(LoginModel loginData)
        {
            string resultMessage = string.Empty;
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7075/api/user/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var responseToken = await response.Content.ReadFromJsonAsync<Auth0TokenResponse>();

                    var returnTokenResult = new Auth0TokenResponse
                    {
                        access_token = responseToken.access_token,
                        refresh_token = responseToken.refresh_token,

                    };

                    return returnTokenResult;
                }
                else
                {
                    resultMessage = "Login unsuccessfully!";
                    Console.WriteLine(resultMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Login unsuccessfully! {ex.Message}";
                Console.WriteLine(resultMessage);
                return null;
            }
        }

        // User signup service okay dont change
        public async Task<bool> RegisterUser(UserModel user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7075/api/user/register", user);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        //// Check if user is authenticated
        //public async Task<bool> IsAuthenticated()
        //{
        //    var accessToken = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", "access_token");
        //    return !string.IsNullOrEmpty(accessToken);
        //}

    }
}

