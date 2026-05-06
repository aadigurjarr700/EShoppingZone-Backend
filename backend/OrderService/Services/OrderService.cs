using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Interfaces;

namespace OrderService.Services
{
    public class OrderServiceImplementation : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderServiceImplementation(IOrderRepository orderRepository, OrderDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IList<Orders>> GetAllOrders()
        {
            return await _orderRepository.GetAllOrders();
        }

        public async Task PlaceOrder(CartDto cart, int customerId, Address address)
        {
            // Start an EF Core transaction to ensure atomicity
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in cart.Items)
                {
                    var order = new Orders
                    {
                        OrderDate = DateTime.UtcNow,
                        CustomerId = customerId,
                        AmountPaid = item.Price * item.Quantity,
                        ModeOfPayment = "COD",
                        OrderStatus = "Placed",
                        Quantity = item.Quantity,
                        Address = new Address
                        {
                            CustomerId = address.CustomerId,
                            FullName = address.FullName,
                            MobileNumber = address.MobileNumber,
                            FlatNumber = address.FlatNumber,
                            City = address.City,
                            Pincode = address.Pincode,
                            State = address.State
                        },
                        Product = new ProductSnapshot
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName
                        }
                    };

                    await _orderRepository.AddOrder(order);

                    // HTTP Call to ProductService to decrement stock
                    // Example: await DecrementProductStock(item.ProductId, item.Quantity);
                    // NOTE: This is a placeholder since ProductService currently lacks a Stock field.
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task OnlinePayment(CartDto cart, int customerId, Address address)
        {
            // Call WalletService before persisting the order
            var walletSuccess = await PayViaWallet(customerId, cart.TotalPrice);
            if (!walletSuccess)
                throw new InvalidOperationException("Insufficient balance in your E-Wallet to complete this order.");

            // Proceed to place order atomically if payment was successful
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var item in cart.Items)
                {
                    var order = new Orders
                    {
                        OrderDate = DateTime.UtcNow,
                        CustomerId = customerId,
                        AmountPaid = item.Price * item.Quantity,
                        ModeOfPayment = "E-Wallet",
                        OrderStatus = "Placed",
                        Quantity = item.Quantity,
                        Address = new Address
                        {
                            CustomerId = address.CustomerId,
                            FullName = address.FullName,
                            MobileNumber = address.MobileNumber,
                            FlatNumber = address.FlatNumber,
                            City = address.City,
                            Pincode = address.Pincode,
                            State = address.State
                        },
                        Product = new ProductSnapshot
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName
                        }
                    };

                    await _orderRepository.AddOrder(order);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<bool> PayViaWallet(int customerId, decimal amount)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", token);
                }

                var walletServiceUrl = _configuration["WalletServiceUrl"] ?? "https://eshopping-wallet.onrender.com";
                
                var response = await client.PostAsync($"{walletServiceUrl}/api/wallet/payMoney?customerId={customerId}&amount={amount}", null);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successfully paid {amount} via WalletService for Customer {customerId}");
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task ChangeStatus(string status, int orderId)
        {
            await _orderRepository.UpdateOrderStatus(orderId, status);
        }

        public async Task DeleteOrder(int orderId)
        {
            await _orderRepository.DeleteOrder(orderId);
        }

        public async Task<IList<Orders>> GetOrderByCustomerId(int customerId)
        {
            return await _orderRepository.FindByCustomerId(customerId);
        }

        public async Task StoreAddress(Address address, int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            if (order != null)
            {
                order.Address = address;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<Address>> GetAddressByCustomerId(int customerId)
        {
            return await _orderRepository.GetAddressByCustomerId(customerId);
        }

        public async Task<IList<Address>> GetAllAddress()
        {
            return await _orderRepository.GetAllAddress();
        }

        public async Task<Orders> GetOrderById(int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            return order;
        }
    }
}
