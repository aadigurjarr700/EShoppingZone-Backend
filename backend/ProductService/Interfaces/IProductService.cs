using System.Collections.Generic;
using System.Threading.Tasks;
using ProductService.Entities;
using ProductService.DTOs;

namespace ProductService.Interfaces
{
    public interface IProductService
    {
        Task AddProducts(AddProductDto productDto);
        Task<IList<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<IList<Product>> GetProductByName(string name);
        Task<IList<Product>> GetProductsByCategory(string category);
        Task<IList<Product>> GetProductsByType(string type);
        Task UpdateProducts(int id, AddProductDto productDto);
        Task DeleteProductById(int id);
    }
}
