using EcomApi.Application.DTO;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EcomApi.Application.Custom_Midleware
{
    public class GlobalCustomMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalCustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


                if (context.Request.Path.StartsWithSegments("/login"))
                {
                    // Return the exception directly without altering the response
                    throw;
                }
                else
                {
                    var errorMessage = new ErrorMessageDTO
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = ex.Message
                    };

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(errorMessage);
                    // Set the response content type to JSON
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(json);

                    Log.Error(ex, "An error occured: {message}", json);
                }
            }
        }
    }
}
