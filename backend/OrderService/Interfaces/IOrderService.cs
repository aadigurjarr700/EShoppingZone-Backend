using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.DTOs;
using OrderService.Entities;

namespace OrderService.Interfaces
{
    public interface IOrderService
    {
        Task<IList<Orders>> GetAllOrders();
        Task PlaceOrder(CartDto cart, int customerId, Address address);
        Task ChangeStatus(string status, int orderId);
        Task DeleteOrder(int orderId);
        Task<IList<Orders>> GetOrderByCustomerId(int customerId);
        
        // We accept the full cart, the customerId (from token), and an Address object 
        Task OnlinePayment(CartDto cart, int customerId, Address address);
        
        Task StoreAddress(Address address, int orderId);
        Task<IList<Address>> GetAddressByCustomerId(int customerId);
        Task<IList<Address>> GetAllAddress();
        Task<Orders> GetOrderById(int orderId);
    }
}
