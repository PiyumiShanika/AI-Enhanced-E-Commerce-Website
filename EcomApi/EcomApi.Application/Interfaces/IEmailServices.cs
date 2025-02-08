using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.Interfaces
{
    public interface IEmailServices
    {
        Task SendEmail(string Name, string Email, string Template, string Subject);
    }
}
