using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Entities;

namespace OrderService.Interfaces
{
    public interface IOrderRepository
    {
        Task<IList<Orders>> FindByCustomerId(int customerId);
        Task<Orders?> FindFirstByOrderByOrderIdDesc();
        Task AddOrder(Orders order);
        Task UpdateOrderStatus(int orderId, string status);
        Task DeleteOrder(int orderId);
        Task<IList<Orders>> GetAllOrders();
        Task<Orders?> GetOrderById(int orderId);
        
        // Since Address is embedded via OwnsOne, getting/storing address is implicitly handled 
        // through Orders, but we can have specific methods if we wanted to extract distinct addresses.
        Task<IList<Address>> GetAddressByCustomerId(int customerId);
        Task<IList<Address>> GetAllAddress();
    }
}
