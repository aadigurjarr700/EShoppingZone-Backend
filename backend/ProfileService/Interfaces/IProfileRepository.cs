using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.Entities;

namespace ProfileService.Interfaces
{
    public interface IProfileRepository
    {
        Task<UserProfile> AddProfileAsync(UserProfile userProfile);
        Task<IList<UserProfile>> GetAllProfilesAsync();
        Task<UserProfile> GetByProfileIdAsync(int profileId);
        Task<UserProfile> FindByMobileNumberAsync(long mobileNumber);
        Task<UserProfile> FindByFullNameAsync(string fullName);
        Task<UserProfile> FindByEmailIdAsync(string emailId);
        Task UpdateProfileAsync(UserProfile userProfile);
        Task DeleteProfileAsync(int profileId);
    }
}
