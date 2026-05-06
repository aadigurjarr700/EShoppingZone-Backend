using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.DTOs;

namespace ProfileService.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileResponseDto> AddCustomerProfile(RegisterDto registerDto);
        Task<ProfileResponseDto> AddMerchantProfile(RegisterDto registerDto);
        Task<ProfileResponseDto> AddAdminProfile(RegisterDto registerDto);
        Task<IList<ProfileResponseDto>> GetAllProfiles();
        Task<ProfileResponseDto> GetByProfileId(int profileId);
        Task<ProfileResponseDto> FindByMobileNo(long mobileNo);
        Task<ProfileResponseDto> GetByUserName(string emailId);
        Task UpdateProfile(int profileId, RegisterDto updateDto);
        Task DeleteProfile(int profileId);
        Task<string> Login(LoginDto loginDto);
        Task ChangePassword(ChangePasswordDto changePasswordDto);
    }
}
