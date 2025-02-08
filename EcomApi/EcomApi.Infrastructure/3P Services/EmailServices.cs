using EcomApi.Application.Interfaces;
using EcomApi.Infrastructure.Config;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Infrastructure._3P_Services
{
    public class EmailServices : IEmailServices
    {

        private readonly KeyConfig _options;

        public EmailServices(IOptions<KeyConfig> options)
        {
            _options = options.Value;
        }
        public async Task SendEmail(string Name, string Email, string Template, string Subject)
        {
            try
            {
                var apiKey = _options.SendGridapiKey;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("tharushathejanofficial@gmail.com", "Shopping Bay");
                var subject = Subject;
                var to = new EmailAddress(Email, Name);
                var plainTextContent = "";
                var htmlContent = Template;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sending email for this User: {Email}", Email);
                throw new Exception("There was an error sending the email. Please try again later.");
            }
        }
    }
}
