using Microsoft.EntityFrameworkCore;
using WalletService.Entities;

namespace WalletService.Data
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options) { }

        public DbSet<EWallet> Wallets { get; set; }
        public DbSet<Statement> Statements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Statement>()
                .HasOne(s => s.Wallet)
                .WithMany(w => w.Statements)
                .HasForeignKey(s => s.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
