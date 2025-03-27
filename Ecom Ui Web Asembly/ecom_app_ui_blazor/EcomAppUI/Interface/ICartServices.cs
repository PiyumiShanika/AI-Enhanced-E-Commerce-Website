using EcomAppUI.Models;

namespace EcomAppUI.Interface
{
    public interface ICartServices
    {
        Task<string> AddToCart(UserCart userCart);
        Task<CartResponse> ViewCart();
        Task<DeleteCartResponse> DeleteCartItem(int productId);
        Task<CartResponse> UpdateCartItemQuantityAsync(int productId, int newQuantity);
        Task<string> CreateCheckoutSession();


    }
}
