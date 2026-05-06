using System.Collections.Generic;
using System.Threading.Tasks;
using WalletService.Entities;

namespace WalletService.Interfaces
{
    public interface IWalletRepository
    {
        Task<IList<EWallet>> GetWallets();
        Task AddWallet(EWallet wallet);
        Task UpdateWallet(EWallet wallet);
        Task<EWallet?> GetById(int walletId);
        Task<IList<Statement>> GetStatementsById(int walletId);
        Task<IList<Statement>> GetStatements();
        Task DeleteById(int walletId);
        Task AddStatement(Statement statement);
    }
}
