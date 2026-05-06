using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProfileService.Data;
using ProfileService.Entities;
using ProfileService.Interfaces;

namespace ProfileService.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;

        public ProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> AddProfileAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<IList<UserProfile>> GetAllProfilesAsync()
        {
            return await _context.UserProfiles
                .Include(u => u.Addresses)
                .ToListAsync();
        }

        public async Task<UserProfile> GetByProfileIdAsync(int profileId)
        {
            return await _context.UserProfiles
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.ProfileId == profileId);
        }

        public async Task<UserProfile> FindByMobileNumberAsync(long mobileNumber)
        {
            return await _context.UserProfiles
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);
        }

        public async Task<UserProfile> FindByFullNameAsync(string fullName)
        {
            return await _context.UserProfiles
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.FullName.ToLower() == fullName.ToLower());
        }

        public async Task<UserProfile> FindByEmailIdAsync(string emailId)
        {
            return await _context.UserProfiles
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.EmailId.ToLower() == emailId.ToLower());
        }

        public async Task UpdateProfileAsync(UserProfile userProfile)
        {
            // The entity is already tracked by the DbContext.
            // Calling Update() on a tracked entity can cause tracking conflicts, especially with child collections.
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProfileAsync(int profileId)
        {
            var profile = await GetByProfileIdAsync(profileId);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }
    }
}
