using System.Collections.Generic;
using System.Threading.Tasks;
using WalletService.Entities;

namespace WalletService.Interfaces
{
    public interface IWalletService
    {
        Task<IList<EWallet>> GetWallets();
        Task AddWallet(EWallet wallet);
        Task AddMoney(EWallet wallet, decimal amount, string remarks);
        Task Update(EWallet wallet, decimal amount, string remarks, int orderId);
        Task<EWallet?> GetById(int walletId);
        Task<IList<Statement>> GetStatementsById(int walletId);
        Task<IList<Statement>> GetStatements();
        Task DeleteById(int walletId);

        // Razorpay Integration
        Task<string> CreateRazorpayOrder(decimal amount);
        Task<bool> VerifyRazorpayPayment(string orderId, string paymentId, string signature);
    }
}
