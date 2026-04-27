using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using ProductService.DTOs;
using ProductService.Entities;
using ProductService.Interfaces;
using ProductService.Services;
using Xunit;

namespace ProductService.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly ProductServiceImplementation _service;

        public ProductServiceTests()
        {
            _repoMock = new Mock<IProductRepository>();
            _service = new ProductServiceImplementation(_repoMock.Object);
        }

        // ────────────────────────────────────────────────────────────
        // Add Product
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task AddProducts_ShouldCallRepositoryOnce()
        {
            var dto = new AddProductDto
            {
                ProductName = "Laptop",
                Category = "Electronics",
                ProductType = "Digital",
                Price = 75000,
                Description = "Fast laptop",
                MerchantId = 10
            };

            _repoMock.Setup(r => r.AddProduct(It.IsAny<Product>())).Returns(Task.CompletedTask);

            await _service.AddProducts(dto);

            _repoMock.Verify(r => r.AddProduct(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task AddProducts_ShouldMapFieldsCorrectly()
        {
            Product capturedProduct = null;
            _repoMock.Setup(r => r.AddProduct(It.IsAny<Product>()))
                     .Callback<Product>(p => capturedProduct = p)
                     .Returns(Task.CompletedTask);

            var dto = new AddProductDto
            {
                ProductName = "Phone",
                Category = "Mobile",
                ProductType = "Electronics",
                Price = 30000,
                MerchantId = 5
            };

            await _service.AddProducts(dto);

            Assert.NotNull(capturedProduct);
            Assert.Equal("Phone", capturedProduct.ProductName);
            Assert.Equal(30000, capturedProduct.Price);
            Assert.Equal(5, capturedProduct.MerchantId);
        }

        // ────────────────────────────────────────────────────────────
        // Get All Products
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "A", Price = 100 },
                new Product { ProductId = 2, ProductName = "B", Price = 200 }
            };
            _repoMock.Setup(r => r.GetAllProducts()).ReturnsAsync(products);

            var result = await _service.GetAllProducts();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllProducts_EmptyRepo_ShouldReturnEmptyList()
        {
            _repoMock.Setup(r => r.GetAllProducts()).ReturnsAsync(new List<Product>());

            var result = await _service.GetAllProducts();

            Assert.Empty(result);
        }

        // ────────────────────────────────────────────────────────────
        // Get Product By Id
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetProductById_ExistingId_ShouldReturnProduct()
        {
            var product = new Product { ProductId = 1, ProductName = "Keyboard", Price = 2000 };
            _repoMock.Setup(r => r.GetProductById(1)).ReturnsAsync(product);

            var result = await _service.GetProductById(1);

            Assert.Equal("Keyboard", result.ProductName);
        }

        [Fact]
        public async Task GetProductById_NotFound_ShouldThrowKeyNotFoundException()
        {
            _repoMock.Setup(r => r.GetProductById(99)).ReturnsAsync((Product)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetProductById(99));
        }

        // ────────────────────────────────────────────────────────────
        // Get By Name / Category / Type / Merchant
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetProductByName_ShouldReturnMatchingProducts()
        {
            var products = new List<Product> { new Product { ProductName = "Phone" } };
            _repoMock.Setup(r => r.FindByProductName("Phone")).ReturnsAsync(products);

            var result = await _service.GetProductByName("Phone");

            Assert.Single(result);
        }

        [Fact]
        public async Task GetProductsByCategory_ShouldReturnMatchingProducts()
        {
            var products = new List<Product>
            {
                new Product { ProductName = "Fan", Category = "Appliance" },
                new Product { ProductName = "AC",  Category = "Appliance" }
            };
            _repoMock.Setup(r => r.FindByCategory("Appliance")).ReturnsAsync(products);

            var result = await _service.GetProductsByCategory("Appliance");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProductsByMerchant_ShouldReturnMerchantProducts()
        {
            var products = new List<Product> { new Product { ProductId = 5, MerchantId = 3 } };
            _repoMock.Setup(r => r.FindByMerchantId(3)).ReturnsAsync(products);

            var result = await _service.GetProductsByMerchant(3);

            Assert.Single(result);
            Assert.Equal(3, result[0].MerchantId);
        }

        // ────────────────────────────────────────────────────────────
        // Update Product
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateProducts_ExistingId_ShouldCallUpdate()
        {
            var existing = new Product
            {
                ProductId = 1, ProductName = "Old Name", Price = 100,
                Rating = new Dictionary<int, double>(), Review = new Dictionary<int, string>()
            };
            _repoMock.Setup(r => r.GetProductById(1)).ReturnsAsync(existing);
            _repoMock.Setup(r => r.UpdateProduct(It.IsAny<Product>())).Returns(Task.CompletedTask);

            var dto = new AddProductDto { ProductName = "New Name", Price = 200, Category = "Cat", ProductType = "Type" };

            await _service.UpdateProducts(1, dto);

            _repoMock.Verify(r => r.UpdateProduct(It.IsAny<Product>()), Times.Once);
            Assert.Equal("New Name", existing.ProductName);
        }

        [Fact]
        public async Task UpdateProducts_NotFound_ShouldThrow()
        {
            _repoMock.Setup(r => r.GetProductById(999)).ReturnsAsync((Product)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateProducts(999, new AddProductDto { ProductName = "X", Category = "Y", ProductType = "Z" }));
        }

        // ────────────────────────────────────────────────────────────
        // Delete Product
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteProductById_ExistingId_ShouldCallDelete()
        {
            var product = new Product { ProductId = 1, ProductName = "To Delete", Price = 500 };
            _repoMock.Setup(r => r.GetProductById(1)).ReturnsAsync(product);
            _repoMock.Setup(r => r.DeleteProductById(1)).Returns(Task.CompletedTask);

            await _service.DeleteProductById(1);

            _repoMock.Verify(r => r.DeleteProductById(1), Times.Once);
        }

        [Fact]
        public async Task DeleteProductById_NotFound_ShouldThrow()
        {
            _repoMock.Setup(r => r.GetProductById(500)).ReturnsAsync((Product)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteProductById(500));
        }
    }
}
