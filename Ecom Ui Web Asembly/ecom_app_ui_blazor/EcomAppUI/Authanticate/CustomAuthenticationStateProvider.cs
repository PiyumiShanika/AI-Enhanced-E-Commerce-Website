using Blazored.SessionStorage;
using EcomAppUI.Interface;
using EcomAppUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    //private readonly ILocalStorageService _localStorage;
    private readonly ISessionStorageService _sessionStorage;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsruntime;
    private readonly NavigationManager _navigationManager;
    private ClaimsPrincipal _principal = new ClaimsPrincipal(new ClaimsIdentity());

    private readonly string ClientId = "BowXYxeRXuSkimSDilCmJga86MGFu9KL";
    private readonly string ClientSecret = "lT-e9mdzVpItjnnNzZ8zvI-S-pl_WfcPh0tEvKAoRJCr3Kjy62J5xiP607bRBNFJ";

    public CustomAuthenticationStateProvider(ISessionStorageService sessionStorge, HttpClient httpClient, NavigationManager navigationManager,IJSRuntime jSRuntime)
    {
       
        _sessionStorage = sessionStorge;
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _jsruntime = jSRuntime;

    }


    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            //localStorage
            //var userLocalStorageResult = await _localStorage.GetItemAsync<string>("access_Token");

            ClaimsPrincipal claimsPrincipal;
            var userSessionStorageResult = await _sessionStorage.GetItemAsync<string>("access_Token");
            var userEmailResult = await _sessionStorage.GetItemAsync<string>("email");
            var userRefreshToken = await _sessionStorage.GetItemAsync<string>("refresh_Token");

            if (IsTokenExpired(userSessionStorageResult) || string.IsNullOrWhiteSpace(userSessionStorageResult))
            {

                if (IsTokenExpired(userSessionStorageResult))
                {
                    var request = $"grant_type=refresh_token&client_id={ClientId}&client_secret={ClientSecret}&refresh_token={userRefreshToken}&scope=openid%20profile";
                    var content = new StringContent(request, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = await _httpClient.PostAsync("https://dev-ofqzic7hcbcpv5tz.us.auth0.com/oauth/token", content);
                    Console.WriteLine(response);
                 


                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var tokenResponse = await response.Content.ReadAsStringAsync();
                        Auth0TokenResponse tokenToJson = JsonConvert.DeserializeObject<Auth0TokenResponse>(tokenResponse);
                        var accessToken = tokenToJson.access_token;
                        var refreshtoken = tokenToJson.refresh_token;

                        var result = new Auth0TokenResponse
                        {
                            access_token = accessToken,
                            refresh_token = refreshtoken,
                        };


                        await _sessionStorage.SetItemAsync("access_Token", accessToken);
                        await _sessionStorage.SetItemAsync("refresh_Token", refreshtoken);

                        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, accessToken),
                            new Claim(ClaimTypes.Authentication, refreshtoken),
                            new Claim(ClaimTypes.Name, userEmailResult),
                        }));

                    }
                    else
                    {
                        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                    }
                }

                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            else
            {
                _principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Email, userSessionStorageResult),
                new Claim(ClaimTypes.Name, userEmailResult)

            }, "CustomAuth"));  //change ponit "CustomAuth"
                return await Task.FromResult(new AuthenticationState(_principal));

            }
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_principal));
        }
    }

    public async Task UpdateAuthenticationState(Auth0TokenResponse token, string email)
    {
        ClaimsPrincipal claimsPrincipal;

        if (token != null)
        {
            await _sessionStorage.SetItemAsync("access_Token", token.access_token);
            await _sessionStorage.SetItemAsync("refresh_Token", token.refresh_token);
            await _sessionStorage.SetItemAsync("email", email);


            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Email, token.access_token),
            new Claim(ClaimTypes.Authentication, token.refresh_token),
            new Claim(ClaimTypes.Name, email),
        }));

        }
        else
        {
            await _sessionStorage.RemoveItemAsync("access_Token");
            await _sessionStorage.RemoveItemAsync("refresh_Token");
            await _sessionStorage.RemoveItemAsync("email");
            claimsPrincipal = _principal;
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }


    private bool IsTokenExpired(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            return true;

        var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (exp == null)
            return true;

        var expDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));

        return expDate < DateTimeOffset.UtcNow;
    }


public async Task Logout()

    {

        // Clear access token from localStorage

        await _jsruntime.InvokeVoidAsync("localStorage.removeItem", "access_token");

        await _jsruntime.InvokeVoidAsync("localStorage.removeItem", "access_token");

        await UpdateAuthenticationState(null, null);

        _navigationManager.NavigateTo("/", true);

    }
}
