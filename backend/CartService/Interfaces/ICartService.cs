using System.Collections.Generic;
using System.Threading.Tasks;
using CartService.Entities;

namespace CartService.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartById(int cartId);
        Task<Cart> UpdateCart(int cartId, int productId, int quantity);
        Task<Cart> SetCartQuantity(int cartId, int productId, int quantity);
        Task<IList<Cart>> GetAllCarts();
        Task<decimal> CartTotal(int cartId);
        Task<Cart> AddCart(int cartId);
        Task<Cart> ClearCart(int cartId);
    }
}
