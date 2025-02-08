using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public string Error { get; set; }

        //add refresh token need to remove
        public static AuthResult Success(string atoken, string rtoken)
        {
            return new AuthResult
            {
                IsSuccess = true,
                Access_Token = atoken,
                Refresh_Token = rtoken
            };
        }

        public static AuthResult Failure(string error)
        {
            return new AuthResult { IsSuccess = false, Error = error };
        }
    }
}
