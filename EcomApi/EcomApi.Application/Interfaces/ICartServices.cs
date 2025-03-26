using EcomApi.Application.DTO;
using EcomApi.Domain.Models;

namespace EcomApi.Application.Interfaces
{
    public interface ICartServices
    {
        Task<UserProduct> AddTocart(CartDTO cartDTOReq);
        Task<UserProduct> deleteCart(int product_id);


        Task<object> View_Cart();


        Task<UserProduct> UpdateCartItemQuantityAsync(int productId, int newQuantity);


    }
}
