//using EcomAppUI.Interface;
//using Microsoft.JSInterop;
//using System.Net.Http.Headers;

//namespace EcomAppUI.Helper
//{
//    public class RefreshTokenHandler : DelegatingHandler
//    {
//        private readonly IJSRuntime _jsRuntime;
//        private readonly IUserServices _userServices;

//        public RefreshTokenHandler(IJSRuntime jsRuntime, IUserServices userServices)
//        {
//            _jsRuntime = jsRuntime;
//            _userServices = userServices;
//        }

//        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "access_token");
//            var expirationTimeString = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "expiration_time");

//            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expirationTimeString))
//            {
//                return await base.SendAsync(request, cancellationToken);
//            }

//            DateTime expirationTime;
//            if (!DateTime.TryParse(expirationTimeString, out expirationTime) || DateTime.UtcNow >= expirationTime)
//            {
//                token = await _userServices.RefreshAccessTokenAsync();
//                var newExpirationTime = DateTime.UtcNow.AddMinutes(15).ToString("o");
//                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "expiration_time", newExpirationTime);
//            }

//            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
//            return await base.SendAsync(request, cancellationToken);
//        }
//    }
//}
