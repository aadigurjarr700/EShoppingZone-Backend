using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Entities;
using OrderService.Interfaces;

namespace OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Orders>> FindByCustomerId(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Orders?> FindFirstByOrderByOrderIdDesc()
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderId)
                .FirstOrDefaultAsync();
        }

        public async Task AddOrder(Orders order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderStatus(int orderId, string status)
        {
            // ExecuteUpdateAsync allows atomic updates without loading the full entity
            await _context.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.OrderStatus, status));
        }

        public async Task DeleteOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<Orders>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Orders?> GetOrderById(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }

        public async Task<IList<Address>> GetAddressByCustomerId(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId && o.Address != null)
                .Select(o => o.Address!)
                .ToListAsync();
        }

        public async Task<IList<Address>> GetAllAddress()
        {
            return await _context.Orders
                .Where(o => o.Address != null)
                .Select(o => o.Address!)
                .ToListAsync();
        }
    }
}
