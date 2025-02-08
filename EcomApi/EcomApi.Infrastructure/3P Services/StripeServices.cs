using EcomApi.Application.Interfaces;
using EcomApi.Domain.Models;
using EcomApi.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Serilog;
using Stripe.Checkout;
using Stripe;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure._3P_Services
{
    public class StripeServices : IStripeServices
    {


        private readonly ITokenService _tokenService;
        private readonly ITransaction _transaction;
        private readonly KeyConfig _options;



        public StripeServices(ITokenService tokenService, IOptions<KeyConfig> options, ITransaction transaction)
        {
            _tokenService = tokenService;
            _transaction = transaction;
            _options = options.Value;

        }

        //Stripe Customer create
        public async Task<Customer> CreateCustomer(string cname, string cemail)
        {
            var email = cemail;
            var name = cname;
            try
            {
                // Set the Stripe API key for authentication
                StripeConfiguration.ApiKey = _options.StripeSecretKey;

                // Create the customer creation options with the provided email and name
                var customer = new CustomerCreateOptions
                {
                    Email = email,
                    Name = name,
                };

                // Instantiate the customer service to interact with the Stripe API
                var service = new CustomerService();

                // Create the customer on Stripe
                var result = service.Create(customer);

                Log.Information("Successfully created Stripe customer with ID: {CustomerId} for email: {Email}", result.Id, email);

                return result;

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating Stripe customer for email: {Email}, name: {Name}", email, name);
                throw new Exception("There was an error creating your account. Please try again later.");
            }
        }


        //stripe checkout service start
        public async Task<string> CreateCheckoutSession()
        {
            try
            {
                var c_user = _tokenService.GetUserId();
                Log.Information("Creating checkout session for user: {UserId}", c_user);

                // Fetch the Stripe customer ID for the current logged-in user
                var stripeCustomerId = await GetStripeCustomerId(c_user);

                // Fetch user cart details from UserProduct table based on customer ID
                var userCart = GetUserCartDetailsForCheckout(c_user);

                // Create line items based on the user cart
                var lineItems = new List<SessionLineItemOptions>();

                foreach (var item in userCart)
                {

                    //check item quntity with stock
                    if (item.Quntity > item.product.Stock)
                    {
                        Log.Warning("Product quantity exceeds available stock. Product: {ProductName}, Quantity: {RequestedQuantity}, Stock: {AvailableStock}",
                                       item.product.Name, item.Quntity, item.product.Stock);

                        // If quantity is greater than stock, display an error message and return
                        return "Error: Your requested quantity of the product " + item.product.Name + " exceeds the available stock.";

                    }


                    // Add the item to the list of line items
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long?)item.product.Price * 100, // Assuming UnitPrice is in cents
                            Currency = "LKR",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Name,

                            },


                        },
                        Quantity = item.Quntity,
                    });
                    Console.WriteLine("hi");
                }

                // Calculate the total amount from the cart
                var totalAmount = userCart.Sum(item => item.product.Price * item.Quntity);

                // Create the session options for Stripe
                var options = new SessionCreateOptions
                {
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = "https://www.itelasoft.com.au/",
                    CancelUrl = "http://localhost:4242/cancel",
                    Customer = stripeCustomerId,

                };

                // Create the checkout session using Stripe's SessionService
                var service = new SessionService();
                Session session = service.Create(options);
                Log.Information("Checkout session created successfully for user: {UserId}", c_user);

                // Open the checkout session URL in the default web browser
                Process.Start(new ProcessStartInfo("cmd", $"/c start {session.Url}")
                {
                    CreateNoWindow = true
                });
                return (session.Url);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating checkout session.");
                throw new Exception("There was an error creating the checkout session. Please try again later.");
            }
        }




        //Retrieve  stripe customer id 
        public async Task<string> GetStripeCustomerId(string c_user)
        {
            try
            {
                Log.Information("Fetching Stripe customer ID for user: {UserId}", c_user);
                var current_user = await _transaction.GetRepository<User>().GetById(c_user, u => u.User_Id == c_user);

                if (current_user != null)
                {
                    Log.Information("Stripe customer ID retrieved successfully for user: {UserId}", c_user);
                    return current_user.Strip_Cust_Id;
                }

                Log.Warning("Stripe customer ID not found for user: {UserId}", c_user);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching Stripe customer ID for user: {UserId}", c_user);
                throw new Exception("Error fetching Stripe customer ID for usePlease try again");
            }
        }


        //Get Cart Details checkoout session
        public List<UserProduct> GetUserCartDetailsForCheckout(string user_Id)
        {
            try
            {
                Log.Information("Fetching cart details for user : {UserId} For Checkout Session", user_Id);

                var userProducts = _transaction.GetRepository<UserProduct>().GetAsyncQueryable(user_Id, up => up.User_ID == user_Id)
                     .Include("product")
                        .Where(up => up.User_ID == user_Id)
                        .ToList();

                Log.Information("Successfully fetched cart details for user : {UserId} For Checkout Session", user_Id);
                return userProducts;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching cart details for user: {UserId} For Checkout Session", user_Id);
                throw new Exception("Error fetching cart details for user For Checkout Session");
            }
        }


        // get payment details
        public async Task<List<PaymentIntent>> GetPaymentDetailsForUser()
        {
            var current_user = _tokenService.GetUserId();
            try
            {

                Log.Information("Fetching payment details for user: {UserId}", current_user);

                // Retrieve the user from the database based on the current user ID

                var user = await _transaction.GetRepository<User>().GetById(current_user, u => u.User_Id == current_user);

                // Retrieve the Stripe customer ID associated with the user
                var strip_id = user.Strip_Cust_Id;

                // Initialize the PaymentIntentService to interact with Stripe
                var paymentIntentService = new PaymentIntentService();

                // Set up options to filter payment intents by customer ID
                var options = new PaymentIntentListOptions
                {
                    Customer = strip_id
                };
                var paymentIntents = await paymentIntentService.ListAsync(options);
                Log.Information("Payment details fetched successfully for user: {UserId}. Total payment intents: {Count}", current_user, paymentIntents.Data.Count);
                return paymentIntents.Data;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching payment details for user: {UserId}", current_user);
                throw new Exception("Error fetching payment details for user");
            }
        }
    }
}
