using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.Interfaces
{
    public interface IStripeServices
    {

        Task<Customer> CreateCustomer(string cname, string cemail);

        Task<string> CreateCheckoutSession();

        //get payment details
        Task<List<PaymentIntent>> GetPaymentDetailsForUser();


    }
}
