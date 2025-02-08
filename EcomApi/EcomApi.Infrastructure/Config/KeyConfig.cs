using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Infrastructure.Config
{
    public class KeyConfig
    {
        public string auth0Domain { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string audience { get; set; }
        public string StripeSecretKey { get; set; }
        public string SendGridapiKey { get; set; }

    }
}
