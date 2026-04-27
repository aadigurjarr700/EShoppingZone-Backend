using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProfileService.DTOs;
using ProfileService.Entities;
using ProfileService.Interfaces;
using BCrypt.Net;

namespace ProfileService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IConfiguration _configuration;

        public ProfileService(IProfileRepository profileRepository, IConfiguration configuration)
        {
            _profileRepository = profileRepository;
            _configuration = configuration;
        }

        public async Task<ProfileResponseDto> AddCustomerProfile(RegisterDto registerDto)
        {
            return await RegisterUser(registerDto, "CUSTOMER");
        }

        public async Task<ProfileResponseDto> AddMerchantProfile(RegisterDto registerDto)
        {
            return await RegisterUser(registerDto, "MERCHANT");
        }

        public async Task<ProfileResponseDto> AddAdminProfile(RegisterDto registerDto)
        {
            return await RegisterUser(registerDto, "ADMIN");
        }

        private async Task<ProfileResponseDto> RegisterUser(RegisterDto registerDto, string role)
        {
            var existingUser = await _profileRepository.FindByEmailIdAsync(registerDto.EmailId);
            if (existingUser != null) throw new Exception("User with this email already exists");

            var userProfile = new UserProfile
            {
                FullName = registerDto.FullName,
                EmailId = registerDto.EmailId,
                MobileNumber = registerDto.MobileNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = role,
                Image = registerDto.Image,
                About = registerDto.About,
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
                Addresses = registerDto.Addresses?.Select(a => new Address
                {
                    HouseNumber = a.HouseNumber,
                    StreetName = a.StreetName,
                    ColonyName = a.ColonyName,
                    City = a.City,
                    State = a.State,
                    Pincode = a.Pincode
                }).ToList() ?? new List<Address>()
            };

            var createdProfile = await _profileRepository.AddProfileAsync(userProfile);
            return MapToResponseDto(createdProfile);
        }

        public async Task<IList<ProfileResponseDto>> GetAllProfiles()
        {
            var profiles = await _profileRepository.GetAllProfilesAsync();
            return profiles.Select(MapToResponseDto).ToList();
        }

        public async Task<ProfileResponseDto> GetByProfileId(int profileId)
        {
            var profile = await _profileRepository.GetByProfileIdAsync(profileId);
            if (profile == null) throw new Exception("Profile not found");
            return MapToResponseDto(profile);
        }

        public async Task<ProfileResponseDto> FindByMobileNo(long mobileNo)
        {
            var profile = await _profileRepository.FindByMobileNumberAsync(mobileNo);
            if (profile == null) throw new Exception("Profile not found");
            return MapToResponseDto(profile);
        }

        public async Task<ProfileResponseDto> GetByUserName(string name)
        {
            var profile = await _profileRepository.FindByEmailIdAsync(name) ?? await _profileRepository.FindByFullNameAsync(name);
            if (profile == null) throw new Exception("Profile not found");
            return MapToResponseDto(profile);
        }

        public async Task UpdateProfile(int profileId, RegisterDto updateDto)
        {
            var existingProfile = await _profileRepository.GetByProfileIdAsync(profileId);
            if (existingProfile == null) throw new Exception("Profile not found");

            existingProfile.FullName = updateDto.FullName ?? existingProfile.FullName;
            existingProfile.MobileNumber = updateDto.MobileNumber != 0 ? updateDto.MobileNumber : existingProfile.MobileNumber;
            existingProfile.Image = updateDto.Image ?? existingProfile.Image;
            existingProfile.About = updateDto.About ?? existingProfile.About;
            existingProfile.DateOfBirth = updateDto.DateOfBirth != default ? updateDto.DateOfBirth : existingProfile.DateOfBirth;
            existingProfile.Gender = updateDto.Gender ?? existingProfile.Gender;

            if (updateDto.Addresses != null && updateDto.Addresses.Any())
            {
                existingProfile.Addresses.Clear();
                foreach (var addr in updateDto.Addresses)
                {
                    existingProfile.Addresses.Add(new Address
                    {
                        HouseNumber = addr.HouseNumber,
                        StreetName = addr.StreetName,
                        ColonyName = addr.ColonyName,
                        City = addr.City,
                        State = addr.State,
                        Pincode = addr.Pincode,
                        ProfileId = profileId
                    });
                }
            }

            await _profileRepository.UpdateProfileAsync(existingProfile);
        }

        public async Task DeleteProfile(int profileId)
        {
            await _profileRepository.DeleteProfileAsync(profileId);
        }

        public async Task<string> Login(LoginDto loginDto)
        {
            var user = await _profileRepository.FindByEmailIdAsync(loginDto.EmailId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            return GenerateJwtToken(user);
        }

        public async Task ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _profileRepository.FindByEmailIdAsync(changePasswordDto.EmailId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid current password");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _profileRepository.UpdateProfileAsync(user);
        }

        private string GenerateJwtToken(UserProfile user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ProfileId.ToString()),
                new Claim(ClaimTypes.Email, user.EmailId),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("CartId", user.ProfileId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ProfileResponseDto MapToResponseDto(UserProfile profile)
        {
            return new ProfileResponseDto
            {
                ProfileId = profile.ProfileId,
                FullName = profile.FullName,
                Image = profile.Image,
                EmailId = profile.EmailId,
                MobileNumber = profile.MobileNumber,
                About = profile.About,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Role = profile.Role,
                Addresses = profile.Addresses?.Select(a => new AddressDto
                {
                    AddressId = a.AddressId,
                    HouseNumber = a.HouseNumber,
                    StreetName = a.StreetName,
                    ColonyName = a.ColonyName,
                    City = a.City,
                    State = a.State,
                    Pincode = a.Pincode
                }).ToList()
            };
        }
    }
}
