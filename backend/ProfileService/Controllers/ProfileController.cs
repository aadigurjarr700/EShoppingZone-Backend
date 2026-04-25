using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.DTOs;
using ProfileService.Interfaces;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("api/profiles")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("addCustomer")]
        public async Task<IActionResult> AddCustomerProfile([FromBody] RegisterDto registerDto)
        {
            var result = await _profileService.AddCustomerProfile(registerDto);
            return Ok(result);
        }

        [HttpPost("addMerchant")]
        public async Task<IActionResult> AddMerchantProfile([FromBody] RegisterDto registerDto)
        {
            var result = await _profileService.AddMerchantProfile(registerDto);
            return Ok(result);
        }

        [HttpPost("addAdmin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddAdminProfile([FromBody] RegisterDto registerDto)
        {
            var result = await _profileService.AddAdminProfile(registerDto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _profileService.Login(loginDto);
            return Ok(new { Token = token });
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            await _profileService.ChangePassword(changePasswordDto);
            return Ok("Password changed successfully.");
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN,MERCHANT")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _profileService.GetAllProfiles();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetByProfileId(int id)
        {
            var profile = await _profileService.GetByProfileId(id);
            return Ok(profile);
        }

        [HttpGet("phone/{phone}")]
        [Authorize(Roles = "ADMIN,MERCHANT")]
        public async Task<IActionResult> GetByPhoneNumber(long phone)
        {
            var profile = await _profileService.FindByMobileNo(phone);
            return Ok(profile);
        }

        [HttpGet("name/{name}")]
        [Authorize(Roles = "ADMIN,MERCHANT")]
        public async Task<IActionResult> GetByUserName(string name)
        {
            var profile = await _profileService.GetByUserName(name);
            return Ok(profile);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] RegisterDto updateDto)
        {
            await _profileService.UpdateProfile(id, updateDto);
            return Ok("Profile updated successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            await _profileService.DeleteProfile(id);
            return Ok("Profile deleted successfully.");
        }
    }
}
