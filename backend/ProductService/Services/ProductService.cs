using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductService.DTOs;
using ProductService.Entities;
using ProductService.Interfaces;

namespace ProductService.Services
{
    public class ProductServiceImplementation : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductServiceImplementation(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddProducts(AddProductDto productDto)
        {
            var product = new Product
            {
                ProductType = productDto.ProductType,
                ProductName = productDto.ProductName,
                Category = productDto.Category,
                Image = productDto.Image,
                Price = productDto.Price,
                Description = productDto.Description,
                Specification = productDto.Specification,
                MerchantId = productDto.MerchantId
            };

            await _productRepository.AddProduct(product);
        }

        public async Task<IList<Product>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            return product;
        }

        public async Task<IList<Product>> GetProductByName(string name)
        {
            return await _productRepository.FindByProductName(name);
        }

        public async Task<IList<Product>> GetProductsByCategory(string category)
        {
            return await _productRepository.FindByCategory(category);
        }

        public async Task<IList<Product>> GetProductsByType(string type)
        {
            return await _productRepository.FindByProductType(type);
        }

        public async Task UpdateProducts(int id, AddProductDto productDto)
        {
            var existingProduct = await GetProductById(id);

            existingProduct.ProductType = productDto.ProductType;
            existingProduct.ProductName = productDto.ProductName;
            existingProduct.Category = productDto.Category;
            existingProduct.Image = productDto.Image;
            existingProduct.Price = productDto.Price;
            existingProduct.Description = productDto.Description;
            existingProduct.Specification = productDto.Specification;

            await _productRepository.UpdateProduct(existingProduct);
        }

        public async Task DeleteProductById(int id)
        {
            await GetProductById(id); // Throws if not found
            await _productRepository.DeleteProductById(id);
        }
    }
}
