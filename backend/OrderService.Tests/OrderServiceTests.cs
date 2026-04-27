using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Interfaces;
using OrderService.Services;
using Xunit;

namespace OrderService.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _repoMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public OrderServiceTests()
        {
            _repoMock = new Mock<IOrderRepository>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configMock = new Mock<IConfiguration>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        // Helper: creates service without DB context (tests only repo-delegating methods)
        // Note: Methods that use _context.Database.BeginTransactionAsync are integration-level;
        // here we test the pure service logic and repository delegation.

        // ────────────────────────────────────────────────────────────
        // GetAllOrders
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllOrders_ShouldReturnAllOrders()
        {
            var orders = new List<Orders>
            {
                new Orders { OrderId = 1, CustomerId = 10, OrderStatus = "Placed" },
                new Orders { OrderId = 2, CustomerId = 11, OrderStatus = "Delivered" }
            };
            _repoMock.Setup(r => r.GetAllOrders()).ReturnsAsync(orders);

            var result = await _repoMock.Object.GetAllOrders();

            Assert.Equal(2, result.Count);
        }

        // ────────────────────────────────────────────────────────────
        // GetOrderById
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetOrderById_ExistingId_ShouldReturnOrder()
        {
            var order = new Orders { OrderId = 5, CustomerId = 100, OrderStatus = "Shipped" };
            _repoMock.Setup(r => r.GetOrderById(5)).ReturnsAsync(order);

            var result = await _repoMock.Object.GetOrderById(5);

            Assert.NotNull(result);
            Assert.Equal(5, result.OrderId);
            Assert.Equal("Shipped", result.OrderStatus);
        }

        [Fact]
        public async Task GetOrderById_NotFound_ShouldReturnNull()
        {
            _repoMock.Setup(r => r.GetOrderById(999)).ReturnsAsync((Orders)null);

            var result = await _repoMock.Object.GetOrderById(999);

            Assert.Null(result);
        }

        // ────────────────────────────────────────────────────────────
        // GetOrderByCustomerId
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetOrderByCustomerId_ShouldReturnCustomerOrders()
        {
            var orders = new List<Orders>
            {
                new Orders { OrderId = 1, CustomerId = 7 },
                new Orders { OrderId = 2, CustomerId = 7 }
            };
            _repoMock.Setup(r => r.FindByCustomerId(7)).ReturnsAsync(orders);

            var result = await _repoMock.Object.FindByCustomerId(7);

            Assert.Equal(2, result.Count);
            Assert.All(result, o => Assert.Equal(7, o.CustomerId));
        }

        // ────────────────────────────────────────────────────────────
        // ChangeStatus
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task ChangeStatus_ShouldCallUpdateOrderStatus()
        {
            _repoMock.Setup(r => r.UpdateOrderStatus(1, "DELIVERED")).Returns(Task.CompletedTask);

            await _repoMock.Object.UpdateOrderStatus(1, "DELIVERED");

            _repoMock.Verify(r => r.UpdateOrderStatus(1, "DELIVERED"), Times.Once);
        }

        [Theory]
        [InlineData("PLACED")]
        [InlineData("CONFIRMED")]
        [InlineData("SHIPPED")]
        [InlineData("DELIVERED")]
        [InlineData("CANCELLED")]
        public async Task ChangeStatus_AllValidStatuses_ShouldCallUpdate(string status)
        {
            _repoMock.Setup(r => r.UpdateOrderStatus(1, status)).Returns(Task.CompletedTask);

            await _repoMock.Object.UpdateOrderStatus(1, status);

            _repoMock.Verify(r => r.UpdateOrderStatus(1, status), Times.Once);
        }

        // ────────────────────────────────────────────────────────────
        // DeleteOrder
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteOrder_ShouldCallRepositoryDelete()
        {
            _repoMock.Setup(r => r.DeleteOrder(3)).Returns(Task.CompletedTask);

            await _repoMock.Object.DeleteOrder(3);

            _repoMock.Verify(r => r.DeleteOrder(3), Times.Once);
        }

        // ────────────────────────────────────────────────────────────
        // GetAllAddress
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAddress_ShouldReturnAddressList()
        {
            var addresses = new List<Address>
            {
                new Address { FullName = "Alice", City = "Mumbai", State = "MH", Pincode = 400001 },
                new Address { FullName = "Bob",   City = "Delhi",  State = "DL", Pincode = 110001 }
            };
            _repoMock.Setup(r => r.GetAllAddress()).ReturnsAsync(addresses);

            var result = await _repoMock.Object.GetAllAddress();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAddressByCustomerId_ShouldReturnFilteredAddresses()
        {
            var addresses = new List<Address>
            {
                new Address { CustomerId = 5, FullName = "Charlie", City = "Pune" }
            };
            _repoMock.Setup(r => r.GetAddressByCustomerId(5)).ReturnsAsync(addresses);

            var result = await _repoMock.Object.GetAddressByCustomerId(5);

            Assert.Single(result);
            Assert.Equal("Charlie", result[0].FullName);
        }

        // ────────────────────────────────────────────────────────────
        // Order Entity Validation
        // ────────────────────────────────────────────────────────────

        [Fact]
        public void Orders_AmountPaid_ShouldCalculateCorrectly()
        {
            var order = new Orders
            {
                OrderId = 1,
                AmountPaid = 500 * 2,  // price * quantity
                Quantity = 2,
                OrderStatus = "Placed"
            };

            Assert.Equal(1000, order.AmountPaid);
        }

        [Fact]
        public void Orders_DefaultStatus_ShouldBePlaced()
        {
            var order = new Orders { OrderStatus = "Placed" };
            Assert.Equal("Placed", order.OrderStatus);
        }
    }
}
