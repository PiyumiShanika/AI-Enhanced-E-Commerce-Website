using EcomApi.Application.DTO;
using EcomApi.Domain.Models;

namespace EcomApi.Application.Interfaces
{
    public interface IOrderServices
    {
        Task<Order> CreateOrder(long price, string Status, string paymentId, string SessionId, string cid);

        //view bill
        Task<List<PaymentHistoryDTO>> viewOrderHistory();

        //view items 
        Task<List<PaymentHistoryItemDTO>> PaymentHistoryProducts(int orderId);


    }
}
