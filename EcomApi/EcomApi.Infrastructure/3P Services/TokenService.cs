using EcomApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Infrastructure._3P_Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {

            try
            {
                // Retrieve the claims identity from the HttpContext's User property
                var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

                // Extract the user ID from the claims if available
                var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
                if (userId != null)
                {
                    Log.Information("User ID fetched successfully: {UserId}", userId);
                    // Split the user ID if it contains a separator, assuming it's in a specific format
                    string[] splitIds = userId.Split("|");
                    return splitIds[1];
                }
                else
                {
                    Log.Warning("User ID not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching user ID.");
                throw new Exception("Error fetching user ID");
            }

        }
    }
}
