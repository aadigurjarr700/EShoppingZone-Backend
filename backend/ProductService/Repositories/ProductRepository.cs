using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Entities;
using ProductService.Interfaces;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IList<Product>> FindByProductName(string name)
        {
            return await _context.Products
                .Where(p => p.ProductName.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<IList<Product>> FindByCategory(string category)
        {
            return await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        public async Task<IList<Product>> FindByProductType(string type)
        {
            return await _context.Products
                .Where(p => p.ProductType.ToLower() == type.ToLower())
                .ToListAsync();
        }

        public async Task<IList<Product>> FindByMerchantId(int merchantId)
        {
            return await _context.Products
                .Where(p => p.MerchantId == merchantId)
                .ToListAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
