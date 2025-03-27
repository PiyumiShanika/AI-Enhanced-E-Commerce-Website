using EcomAppUI.Models;
using EcomAppUI.Pages;

namespace EcomAppUI.Interface
{
    public interface IOrderServics
    {
        Task<List<PaymentHistory>> ViewOrders();
        Task<List<PaymentHistroyItem>> ViewOrderitems(int order_Id);
    }
}
