using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using EcomApi.Application.Interfaces;
using Stripe.Climate;

namespace EcomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class paymentController : ControllerBase
    {
        private readonly IOrderServices _orderServices;
        private readonly IStripeServices _stripeServices;

        public paymentController(IOrderServices orderServices, IStripeServices stripeServices)
        {
            _orderServices = orderServices;
            _stripeServices = stripeServices;
        }

        /// <summary>
        /// Handles incoming webhook events from Stripe, specifically for the CheckoutSessionCompleted event.
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {


            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();


            try
            {

                var stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);

                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    Console.WriteLine(session.Id);
                    var options = new SessionGetOptions();
                    options.AddExpand("line_items");
                    options.AddExpand("line_items.data.price.product");
                    var service = new SessionService();
                    // Retrieve the session
                    Session sessionWithLineItems = service.Get(session.Id, options);
                    StripeList<LineItem> lineItems = sessionWithLineItems.LineItems;
                    Console.WriteLine(sessionWithLineItems);
                    Console.WriteLine(lineItems);
                    var session_id = sessionWithLineItems.Id;

                    Console.WriteLine($"id is :{session_id}");
                    Console.WriteLine($"Total  :{session.AmountSubtotal}");
                    Console.WriteLine($"Checkout status is :{session.Status}");
                    Console.WriteLine($"payment intent id is :{session.PaymentIntentId}");
                    Console.WriteLine($"payment status id is :{session.PaymentStatus}");
                    var customet_id = session.CustomerId;
                    Console.WriteLine($"payment status id is :{customet_id}");

                    //calling the services save order in order table
                    await _orderServices.CreateOrder((long)session.AmountSubtotal, session.PaymentStatus, session.PaymentIntentId, session_id, customet_id);

                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        //Stripe payment

        /// <summary>
        /// Initiates the creation of a checkout session for purchasing items in the user's shopping cart.
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpPost("purchase-cart")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            try
            {
                var response = await _stripeServices.CreateCheckoutSession();

                return Ok(response);

            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while creating the checkout session.");
            }

        }

        /// <summary>
        /// Retrieves the payment history of the authenticated user.
        /// </summary>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpGet("payment-history")]
        public async Task<IActionResult> ViewPaymentHistory()
        {
            try
            {
                var orderHistory = await _orderServices.viewOrderHistory();
                return Ok(orderHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        /// <summary>
        /// Retrieves the item history of a specific order in the payment history of the authenticated user.
        /// </summary>
        /// <param name="order_Id">The ID of the order for which item history is to be retrieved.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [Authorize]
        [HttpGet("payment-item-history/{order_Id}")]
        public async Task<IActionResult> viewPaymentItemHistory(int order_Id)
        {
            try
            {
                var itemHistory = await _orderServices.PaymentHistoryProducts(order_Id);
                return Ok(itemHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

