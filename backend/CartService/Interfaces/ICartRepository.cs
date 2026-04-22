using System.Collections.Generic;
using System.Threading.Tasks;
using CartService.Entities;

namespace CartService.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> FindByCartId(int cartId);
        Task AddCart(Cart cart);
        Task UpdateCart(Cart cart);
        Task<IList<Cart>> GetAllCarts();
    }
}
