using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletService.Data;
using WalletService.Entities;
using WalletService.Interfaces;

namespace WalletService.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletDbContext _context;

        public WalletRepository(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IList<EWallet>> GetWallets()
        {
            return await _context.Wallets.Include(w => w.Statements).ToListAsync();
        }

        public async Task AddWallet(EWallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWallet(EWallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<EWallet?> GetById(int walletId)
        {
            return await _context.Wallets
                .Include(w => w.Statements)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public async Task<IList<Statement>> GetStatementsById(int walletId)
        {
            return await _context.Statements
                .Where(s => s.WalletId == walletId)
                .OrderByDescending(s => s.DateTime)
                .ToListAsync();
        }

        public async Task<IList<Statement>> GetStatements()
        {
            return await _context.Statements
                .OrderByDescending(s => s.DateTime)
                .ToListAsync();
        }

        public async Task DeleteById(int walletId)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddStatement(Statement statement)
        {
            _context.Statements.Add(statement);
            await _context.SaveChangesAsync();
        }
    }
}
