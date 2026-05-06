using Microsoft.EntityFrameworkCore;
using ProfileService.Entities;

namespace ProfileService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.EmailId)
                .IsUnique();

            modelBuilder.Entity<UserProfile>()
                .HasIndex(u => u.MobileNumber)
                .IsUnique();

            modelBuilder.Entity<Address>()
                .HasOne(a => a.UserProfile)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Map DateOfBirth as a PostgreSQL 'date' (no time/timezone) to avoid Npgsql UTC requirement
            modelBuilder.Entity<UserProfile>()
                .Property(u => u.DateOfBirth)
                .HasColumnType("date");

            // Seed Admin User
            modelBuilder.Entity<UserProfile>().HasData(new UserProfile
            {
                ProfileId = 999,
                FullName = "AadiAdmin",
                EmailId = "aadiadmin123@gmail.com",
                MobileNumber = 1234567890,
                Password = BCrypt.Net.BCrypt.HashPassword("AadiAdmin#123"),
                Role = "ADMIN",
                Image = "admin.png",
                About = "Platform Administrator",
                DateOfBirth = DateTime.SpecifyKind(new System.DateTime(1990, 1, 1), DateTimeKind.Utc),
                Gender = "Male"
            });
        }
    }
}
