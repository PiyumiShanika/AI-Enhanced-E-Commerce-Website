using EcomApi.Application.DTO;
using EcomApi.Application.Services;
using EcomApi.Domain.Models;

namespace EcomApi.Application.Interfaces
{
    public interface IAuthServices
    {
        Task<ServiceResult<User>> registerUser(UserCreateDTO UserRequest);

        //login
        Task<AuthResult> loginUser(UserLoginDTO loginCredentials);
    }
}
