using EcomApi.Application.Interfaces;
using EcomApi.Domain.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using EcomApi.Infrastructure.Config;
using EcomApi.Application.Services;
using EcomApi.Application.DTO;
using EcomApi.Domain.EmailTemplate;

namespace EcomApi.Infrastructure._3P_Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IStripeServices _stripeServices;
        private readonly IEmailServices _emailServices;
        private readonly ITransaction _transaction;
        private readonly KeyConfig _authConfig;

        //for testing purpose
        private HttpClient _client;

        public AuthServices(IStripeServices stripeServices, IEmailServices emailServices, IOptions<KeyConfig> options, ITransaction transaction)
        {

            _stripeServices = stripeServices;
            _emailServices = emailServices;
            _transaction = transaction;
            _authConfig = options.Value;
            _client = new HttpClient(); // default client
        }

        // Method to register a new user
        public async Task<ServiceResult<User>> registerUser(UserCreateDTO UserRequest)
        {
            try
            {
                using var transaction = _transaction.BeginTransactionAsync();

                // Prepare data for Auth0 signup
                var auth0Data = new
                {

                    clientId = _authConfig.clientId,
                    email = UserRequest.Email,
                    password = UserRequest.Password,
                    connection = "Username-Password-Authentication"
                };

                // Convert data to JSON
                var jsonPayload = JObject.FromObject(auth0Data).ToString();
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                Log.Information("Sending request to Auth0 to create user: {Email}", UserRequest.Email);

                // Make HTTP request to Auth0 signup endpoint
                var response = await _client.PostAsync("https://dev-ofqzic7hcbcpv5tz.us.auth0.com/dbconnections/signup", content);

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error("Failed to create user in Auth0 for email: {Email}, Status Code: {StatusCode}", UserRequest.Email, response.StatusCode);
                    return ServiceResult<User>.FailureResult("Failed to create user in Auth0.");
                }

                // Get the Auth0 user ID
                var authId = await response.Content.ReadAsStringAsync();
                AuthDTO authData = JsonConvert.DeserializeObject<AuthDTO>(authId);
                var user_ID = authData._id;
                Log.Information("Auth0 user created successfully with ID: {AuthID}", user_ID);

                // Create Stripe customer
                var stripe_cust = await _stripeServices.CreateCustomer(UserRequest.first_Name, UserRequest.Email);
                Log.Information("Stripe customer created successfully for email: {Email}", UserRequest.Email);


                // Create new User entity
                var user = new User
                {
                    User_Id = user_ID,
                    first_Name = UserRequest.first_Name,
                    Last_Name = UserRequest.Last_Name,
                    Email = UserRequest.Email,
                    Mobile = UserRequest.Mobile,
                    Strip_Cust_Id = stripe_cust.Id

                };

                var user_response = await _transaction.GetRepository<User>().AddAsync(user);
                await _transaction.SaveChangesAsync();


                // If user added successfully, add primary address
                if (user_response != null)
                {
                    var address = new Domain.Models.Address
                    {
                        User_Id = user_ID,
                        Country = UserRequest.Address.Country,
                        City = UserRequest.Address.City,
                        Street = UserRequest.Address.Street,
                        Postal_Code = UserRequest.Address.Postal_Code,
                        isPrimary = true
                    };

                    await _transaction.GetRepository<Address>().AddAsync(address);
                    await _transaction.SaveChangesAsync();
                    await _transaction.CommitAsync();
                    Log.Information("Primary address added for user ID: {UserID}", user_ID);
                }

                Log.Information("User registered successfully with ID: {UserID}", user_ID);

                // Send welcome email to the user

                var template = Template.emailTemplatwelcomee(UserRequest.first_Name);
                var User_Name = UserRequest.first_Name;
                var User_Email = UserRequest.Email;
                var Subject = "Welcome To Shopping Bay";

                await _emailServices.SendEmail(User_Name, User_Email, template, Subject);
                Log.Information("WelCome Email Send To the User: {Email}", UserRequest.Email);

                return ServiceResult<User>.SuccessResult(user);
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                Log.Error(ex, "Error occurred while registering user with email: {Email}", UserRequest.Email);
                throw new Exception("There was an error during user registration. Please try again later.");
            }

        }

        // Method to authenticate and login a user
        public async Task<AuthResult> loginUser(UserLoginDTO loginCredentials)
        {
            try
            {

                // Prepare data for Auth0 authentication
                var auth0Data = new
                {
                    grant_type = "password",
                    username = loginCredentials.Email,
                    password = loginCredentials.Password,
                    client_id = _authConfig.clientId,
                    client_secret = _authConfig.clientSecret,
                    audience = _authConfig.audience,
                    scope = "openid profile email offline_access"
                };



                var json = JObject.FromObject(auth0Data).ToString();
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Create a new HttpClient
                var httpclient = new HttpClient();


                // Send HTTP POST request to Auth0 token endpoint
                var response = await _client.PostAsync($"https://dev-ofqzic7hcbcpv5tz.us.auth0.com/oauth/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<Auth0TokenResponseDTO>(responseContent);
                    Log.Information("Login successful for email: {Email}", loginCredentials.Email);

                    return AuthResult.Success(tokenResponse.access_token, tokenResponse.refresh_token);


                }
                else
                {
                    Log.Warning("Login failed for email: {Email}, Status Code: {StatusCode}", loginCredentials.Email, response.StatusCode);
                    return AuthResult.Failure("Invalid credentials");

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while logging in for email: {Email}", loginCredentials.Email);
                throw new Exception("There was an error during login. Please try again later.");
            }
        }

        //refresh token services
        //public async Task<Auth0TokenResponseDTO> RefreshTokenAsync(string refreshToken)
        //{
        //    var auth0Data = new
        //    {
        //        grant_type = "refresh_token",
        //        client_id = _authConfig.clientId,
        //        client_secret = _authConfig.clientSecret,
        //        refresh_token = refreshToken
        //    };

        //    var json = JObject.FromObject(auth0Data).ToString();
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _client.PostAsync($"https://dev-ofqzic7hcbcpv5tz.us.auth0.com/oauth/token", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var tokenResponse = JsonConvert.DeserializeObject<Auth0TokenResponseDTO>(responseContent);
        //        return tokenResponse;
        //    }
        //    else
        //    {
        //        throw new Exception("Invalid refresh token");
        //    }
        //}


        //sample
        public void SetHttpClient(HttpClient client)
        {
            _client = client;
        }

    }
}
