using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CartService.Entities;
using CartService.Interfaces;
using CartService.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CartService.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _repoMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly CartServiceImplementation _service;

        public CartServiceTests()
        {
            _repoMock = new Mock<ICartRepository>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configMock = new Mock<IConfiguration>();

            _service = new CartServiceImplementation(
                _repoMock.Object,
                _httpClientFactoryMock.Object,
                _configMock.Object);
        }

        // ────────────────────────────────────────────────────────────
        // AddCart
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task AddCart_NewCart_ShouldCreateAndReturnCart()
        {
            _repoMock.Setup(r => r.FindByCartId(1)).ReturnsAsync((Cart)null);
            _repoMock.Setup(r => r.AddCart(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            var result = await _service.AddCart(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.CartId);
            Assert.Equal(0, result.TotalPrice);
        }

        [Fact]
        public async Task AddCart_ExistingCart_ShouldReturnExistingCart()
        {
            var existing = new Cart { CartId = 2, TotalPrice = 500, Items = new List<CartItem>() };
            _repoMock.Setup(r => r.FindByCartId(2)).ReturnsAsync(existing);

            var result = await _service.AddCart(2);

            Assert.Equal(500, result.TotalPrice);
            // Should not call AddCart again
            _repoMock.Verify(r => r.AddCart(It.IsAny<Cart>()), Times.Never);
        }

        // ────────────────────────────────────────────────────────────
        // GetCartById
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetCartById_ExistingCart_ShouldReturnIt()
        {
            var cart = new Cart { CartId = 3, TotalPrice = 100, Items = new List<CartItem>() };
            _repoMock.Setup(r => r.FindByCartId(3)).ReturnsAsync(cart);

            var result = await _service.GetCartById(3);

            Assert.Equal(3, result.CartId);
        }

        [Fact]
        public async Task GetCartById_NotFound_ShouldAutoCreateCart()
        {
            // First call returns null (not found), after AddCart is called it should return new cart
            _repoMock.SetupSequence(r => r.FindByCartId(99))
                     .ReturnsAsync((Cart)null)
                     .ReturnsAsync(new Cart { CartId = 99, TotalPrice = 0, Items = new List<CartItem>() });
            _repoMock.Setup(r => r.AddCart(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            var result = await _service.GetCartById(99);

            Assert.Equal(99, result.CartId);
        }

        // ────────────────────────────────────────────────────────────
        // CartTotal
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task CartTotal_ShouldReturnSumOfItemPrices()
        {
            var cart = new Cart
            {
                CartId = 4,
                TotalPrice = 300,
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Price = 100, Quantity = 2 },
                    new CartItem { ProductId = 2, Price = 50,  Quantity = 2 }
                }
            };
            _repoMock.Setup(r => r.FindByCartId(4)).ReturnsAsync(cart);

            var total = await _service.CartTotal(4);

            Assert.Equal(300, total); // 100*2 + 50*2
        }

        // ────────────────────────────────────────────────────────────
        // ClearCart
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task ClearCart_ShouldRemoveAllItemsAndZeroTotal()
        {
            var cart = new Cart
            {
                CartId = 5,
                TotalPrice = 500,
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Price = 200, Quantity = 1 },
                    new CartItem { ProductId = 2, Price = 300, Quantity = 1 }
                }
            };
            _repoMock.Setup(r => r.FindByCartId(5)).ReturnsAsync(cart);
            _repoMock.Setup(r => r.UpdateCart(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            var result = await _service.ClearCart(5);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalPrice);
        }

        // ────────────────────────────────────────────────────────────
        // SetCartQuantity
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task SetCartQuantity_ExistingItem_ShouldUpdateQuantity()
        {
            var item = new CartItem { ProductId = 1, Price = 100, Quantity = 3, CartId = 6 };
            var cart = new Cart { CartId = 6, TotalPrice = 300, Items = new List<CartItem> { item } };

            _repoMock.Setup(r => r.FindByCartId(6)).ReturnsAsync(cart);
            _repoMock.Setup(r => r.UpdateCart(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            var result = await _service.SetCartQuantity(6, 1, 5);

            Assert.Equal(5, result.Items.First(i => i.ProductId == 1).Quantity);
        }

        [Fact]
        public async Task SetCartQuantity_ZeroQuantity_ShouldRemoveItem()
        {
            var item = new CartItem { ProductId = 2, Price = 50, Quantity = 2, CartId = 7 };
            var cart = new Cart { CartId = 7, TotalPrice = 100, Items = new List<CartItem> { item } };

            _repoMock.Setup(r => r.FindByCartId(7)).ReturnsAsync(cart);
            _repoMock.Setup(r => r.UpdateCart(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            var result = await _service.SetCartQuantity(7, 2, 0);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalPrice);
        }

        // ────────────────────────────────────────────────────────────
        // GetAllCarts
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllCarts_ShouldReturnAllCarts()
        {
            var carts = new List<Cart>
            {
                new Cart { CartId = 1 },
                new Cart { CartId = 2 }
            };
            _repoMock.Setup(r => r.GetAllCarts()).ReturnsAsync(carts);

            var result = await _service.GetAllCarts();

            Assert.Equal(2, result.Count);
        }
    }
}
