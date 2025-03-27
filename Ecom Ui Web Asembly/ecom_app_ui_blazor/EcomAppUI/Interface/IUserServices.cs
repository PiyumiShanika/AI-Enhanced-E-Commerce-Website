using EcomAppUI.Models;

namespace EcomAppUI.Interface
{
    public interface IUserServices
    {
        Task<Auth0TokenResponse> GetAccessToken(LoginModel loginData);
        Task<bool> RegisterUser(UserModel user);
    }
}
