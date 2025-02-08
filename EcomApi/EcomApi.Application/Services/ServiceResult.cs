using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }

        public static ServiceResult<T> SuccessResult(T data)
        {
            return new ServiceResult<T> { Success = true, Data = data };
        }

        public static ServiceResult<T> FailureResult(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage };
        }



    }
}
