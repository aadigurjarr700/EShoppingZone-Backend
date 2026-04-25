using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CartService.DTOs;
using CartService.Entities;
using CartService.Interfaces;

namespace CartService.Services
{
    public class CartServiceImplementation : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CartServiceImplementation(ICartRepository cartRepository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _cartRepository = cartRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<Cart> AddCart(int cartId)
        {
            var existingCart = await _cartRepository.FindByCartId(cartId);
            if (existingCart != null)
                return existingCart; // Cart already exists for this user

            var cart = new Cart { CartId = cartId, TotalPrice = 0, Items = new List<CartItem>() };
            await _cartRepository.AddCart(cart);
            return cart;
        }

        public async Task<Cart> GetCartById(int cartId)
        {
            var cart = await _cartRepository.FindByCartId(cartId);
            if (cart == null)
            {
                // Auto-create cart if it doesn't exist
                cart = await AddCart(cartId);
            }
            return cart;
        }

        public async Task<decimal> CartTotal(int cartId)
        {
            var cart = await GetCartById(cartId);
            return cart.Items.Sum(i => i.Price * i.Quantity);
        }

        public async Task<IList<Cart>> GetAllCarts()
        {
            return await _cartRepository.GetAllCarts();
        }

        public async Task<Cart> UpdateCart(int cartId, int productId, int quantity)
        {
            var cart = await GetCartById(cartId);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (quantity <= 0)
            {
                // Remove item if quantity is 0 or less
                if (existingItem != null)
                {
                    cart.Items.Remove(existingItem);
                }
            }
            else
            {
                if (existingItem != null)
                {
                    // Increment existing item quantity
                    existingItem.Quantity += quantity;
                }
                else
                {
                    // Fetch product details via HTTP call to ProductService
                    var productInfo = await FetchProductDetails(productId);
                    if (productInfo == null)
                        throw new Exception($"Product with ID {productId} does not exist in ProductService.");

                    // Add new item
                    cart.Items.Add(new CartItem
                    {
                        ProductId = productInfo.ProductId,
                        ProductName = productInfo.ProductName,
                        Price = productInfo.Price,
                        Quantity = quantity,
                        CartId = cart.CartId
                    });
                }
            }

            // Recalculate Total
            cart.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

            await _cartRepository.UpdateCart(cart);
            return cart;
        }

        public async Task<Cart> SetCartQuantity(int cartId, int productId, int quantity)
        {
            var cart = await GetCartById(cartId);
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (quantity <= 0)
            {
                if (existingItem != null)
                {
                    cart.Items.Remove(existingItem);
                }
            }
            else
            {
                if (existingItem != null)
                {
                    existingItem.Quantity = quantity;
                }
            }

            cart.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            await _cartRepository.UpdateCart(cart);
            return cart;
        }

        public async Task<Cart> ClearCart(int cartId)
        {
            var cart = await GetCartById(cartId);
            cart.Items.Clear();
            cart.TotalPrice = 0;
            await _cartRepository.UpdateCart(cart);
            return cart;
        }

        private async Task<ProductResponseDto?> FetchProductDetails(int productId)
        {
            var client = _httpClientFactory.CreateClient();
            var productServiceUrl = _configuration["ProductServiceUrl"];
            
            var response = await client.GetAsync($"{productServiceUrl}/api/products/{productId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductResponseDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
