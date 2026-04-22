using System.Collections.Generic;
using System.Threading.Tasks;
using ProductService.Entities;

namespace ProductService.Interfaces
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task<IList<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<IList<Product>> FindByProductName(string name);
        Task<IList<Product>> FindByCategory(string category);
        Task<IList<Product>> FindByProductType(string type);
        Task UpdateProduct(Product product);
        Task DeleteProductById(int id);
    }
}
