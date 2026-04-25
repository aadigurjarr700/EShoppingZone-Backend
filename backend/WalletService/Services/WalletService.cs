using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletService.Data;
using WalletService.Entities;
using WalletService.Interfaces;

namespace WalletService.Services
{
    public class WalletServiceImplementation : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly WalletDbContext _context;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public WalletServiceImplementation(IWalletRepository walletRepository, WalletDbContext context, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _walletRepository = walletRepository;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IList<EWallet>> GetWallets()
        {
            return await _walletRepository.GetWallets();
        }

        public async Task AddWallet(EWallet wallet)
        {
            var existing = await _walletRepository.GetById(wallet.WalletId);
            if (existing == null)
            {
                await _walletRepository.AddWallet(wallet);
            }
        }

        public async Task AddMoney(EWallet wallet, decimal amount, string remarks)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                wallet.CurrentBalance += amount;
                await _walletRepository.UpdateWallet(wallet);

                var statement = new Statement
                {
                    WalletId = wallet.WalletId,
                    TransactionType = "CREDIT",
                    Amount = amount,
                    DateTime = DateTime.UtcNow,
                    TransactionRemarks = remarks
                };
                await _walletRepository.AddStatement(statement);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PayMoney logic implemented using Update to match interface
        public async Task Update(EWallet wallet, decimal amount, string remarks, int orderId)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (wallet.CurrentBalance < amount)
                    throw new InvalidOperationException("Insufficient funds.");

                wallet.CurrentBalance -= amount;
                await _walletRepository.UpdateWallet(wallet);

                var statement = new Statement
                {
                    WalletId = wallet.WalletId,
                    TransactionType = "DEBIT",
                    Amount = amount,
                    DateTime = DateTime.UtcNow,
                    OrderId = orderId,
                    TransactionRemarks = remarks
                };
                await _walletRepository.AddStatement(statement);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<EWallet?> GetById(int walletId)
        {
            return await _walletRepository.GetById(walletId);
        }

        public async Task<IList<Statement>> GetStatementsById(int walletId)
        {
            return await _walletRepository.GetStatementsById(walletId);
        }

        public async Task<IList<Statement>> GetStatements()
        {
            return await _walletRepository.GetStatements();
        }

        public async Task DeleteById(int walletId)
        {
            await _walletRepository.DeleteById(walletId);
        }

        public Task<string> CreateRazorpayOrder(decimal amount)
        {
            var keyId = _configuration["Razorpay:KeyId"];
            var keySecret = _configuration["Razorpay:KeySecret"];
            
            var client = new Razorpay.Api.RazorpayClient(keyId, keySecret);
            
            // Amount in paise (multiply by 100)
            var options = new System.Collections.Generic.Dictionary<string, object>
            {
                { "amount", amount * 100 },
                { "currency", "INR" },
                { "receipt", $"rcpt_{Guid.NewGuid().ToString().Substring(0, 8)}" }
            };

            Razorpay.Api.Order order = client.Order.Create(options);
            return Task.FromResult(order["id"].ToString());
        }

        public Task<bool> VerifyRazorpayPayment(string orderId, string paymentId, string signature)
        {
            var keySecret = _configuration["Razorpay:KeySecret"];
            
            var payload = $"{orderId}|{paymentId}";
            var secretBytes = System.Text.Encoding.UTF8.GetBytes(keySecret);
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            
            using var hmac = new System.Security.Cryptography.HMACSHA256(secretBytes);
            var hashBytes = hmac.ComputeHash(payloadBytes);
            var generatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return Task.FromResult(generatedSignature == signature);
        }
    }
}
