using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using ProfileService.DTOs;
using ProfileService.Entities;
using ProfileService.Interfaces;
using ProfileService.Services;
using Xunit;

namespace ProfileService.Tests
{
    public class ProfileServiceTests
    {
        private readonly Mock<IProfileRepository> _repoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly ProfileService.Services.ProfileService _service;

        public ProfileServiceTests()
        {
            _repoMock = new Mock<IProfileRepository>();
            _configMock = new Mock<IConfiguration>();

            // JWT config stubs
            _configMock.Setup(c => c["Jwt:Key"]).Returns("SuperSecretTestKeyForJwtTokenGeneration123!");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            _service = new ProfileService.Services.ProfileService(_repoMock.Object, _configMock.Object);
        }

        // ────────────────────────────────────────────────────────────
        // Register Tests
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task AddCustomerProfile_ShouldReturnProfileResponseDto()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FullName = "Test Customer",
                EmailId = "customer@test.com",
                Password = "password123",
                MobileNumber = 9876543210,
                Gender = "Male",
                DateOfBirth = new DateTime(2000, 1, 1),
                Role = "CUSTOMER"
            };

            var savedProfile = new UserProfile
            {
                ProfileId = 1,
                FullName = dto.FullName,
                EmailId = dto.EmailId,
                Role = "CUSTOMER",
                Addresses = new List<Address>()
            };

            _repoMock.Setup(r => r.FindByEmailIdAsync(dto.EmailId)).ReturnsAsync((UserProfile)null);
            _repoMock.Setup(r => r.AddProfileAsync(It.IsAny<UserProfile>())).ReturnsAsync(savedProfile);

            // Act
            var result = await _service.AddCustomerProfile(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Customer", result.FullName);
            Assert.Equal("CUSTOMER", result.Role);
        }

        [Fact]
        public async Task AddCustomerProfile_DuplicateEmail_ShouldThrowException()
        {
            // Arrange
            var dto = new RegisterDto { EmailId = "duplicate@test.com", Password = "pass", FullName = "Dup" };
            _repoMock.Setup(r => r.FindByEmailIdAsync(dto.EmailId))
                     .ReturnsAsync(new UserProfile { EmailId = dto.EmailId });

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.AddCustomerProfile(dto));
        }

        [Fact]
        public async Task AddMerchantProfile_ShouldReturnMerchantRole()
        {
            var dto = new RegisterDto
            {
                FullName = "Merchant One",
                EmailId = "merchant@test.com",
                Password = "pass",
                MobileNumber = 1234567890,
                Gender = "Female",
                DateOfBirth = DateTime.Now
            };

            var saved = new UserProfile
            {
                ProfileId = 2,
                FullName = dto.FullName,
                EmailId = dto.EmailId,
                Role = "MERCHANT",
                Addresses = new List<Address>()
            };

            _repoMock.Setup(r => r.FindByEmailIdAsync(dto.EmailId)).ReturnsAsync((UserProfile)null);
            _repoMock.Setup(r => r.AddProfileAsync(It.IsAny<UserProfile>())).ReturnsAsync(saved);

            var result = await _service.AddMerchantProfile(dto);

            Assert.Equal("MERCHANT", result.Role);
        }

        // ────────────────────────────────────────────────────────────
        // Get Tests
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByProfileId_ExistingId_ShouldReturnProfile()
        {
            var profile = new UserProfile
            {
                ProfileId = 5,
                FullName = "Jane Doe",
                EmailId = "jane@test.com",
                Role = "CUSTOMER",
                Addresses = new List<Address>()
            };
            _repoMock.Setup(r => r.GetByProfileIdAsync(5)).ReturnsAsync(profile);

            var result = await _service.GetByProfileId(5);

            Assert.Equal("Jane Doe", result.FullName);
            Assert.Equal(5, result.ProfileId);
        }

        [Fact]
        public async Task GetByProfileId_NotFound_ShouldThrowException()
        {
            _repoMock.Setup(r => r.GetByProfileIdAsync(99)).ReturnsAsync((UserProfile)null);

            await Assert.ThrowsAsync<Exception>(() => _service.GetByProfileId(99));
        }

        [Fact]
        public async Task GetAllProfiles_ShouldReturnList()
        {
            var profiles = new List<UserProfile>
            {
                new UserProfile { ProfileId = 1, FullName = "A", EmailId = "a@test.com", Role = "CUSTOMER", Addresses = new List<Address>() },
                new UserProfile { ProfileId = 2, FullName = "B", EmailId = "b@test.com", Role = "MERCHANT", Addresses = new List<Address>() }
            };
            _repoMock.Setup(r => r.GetAllProfilesAsync()).ReturnsAsync(profiles);

            var result = await _service.GetAllProfiles();

            Assert.Equal(2, result.Count);
        }

        // ────────────────────────────────────────────────────────────
        // Login Tests
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_ShouldReturnJwtToken()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
            var user = new UserProfile
            {
                ProfileId = 1,
                EmailId = "user@test.com",
                Password = hashedPassword,
                Role = "CUSTOMER",
                Addresses = new List<Address>()
            };

            _repoMock.Setup(r => r.FindByEmailIdAsync("user@test.com")).ReturnsAsync(user);

            var token = await _service.Login(new LoginDto { EmailId = "user@test.com", Password = "password123" });

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_InvalidPassword_ShouldThrowUnauthorized()
        {
            var user = new UserProfile
            {
                EmailId = "user@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
                Addresses = new List<Address>()
            };
            _repoMock.Setup(r => r.FindByEmailIdAsync("user@test.com")).ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.Login(new LoginDto { EmailId = "user@test.com", Password = "wrongpassword" }));
        }

        [Fact]
        public async Task Login_UserNotFound_ShouldThrowUnauthorized()
        {
            _repoMock.Setup(r => r.FindByEmailIdAsync("ghost@test.com")).ReturnsAsync((UserProfile)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.Login(new LoginDto { EmailId = "ghost@test.com", Password = "any" }));
        }

        // ────────────────────────────────────────────────────────────
        // Delete Test
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteProfile_ShouldCallRepository()
        {
            _repoMock.Setup(r => r.DeleteProfileAsync(3)).Returns(Task.CompletedTask);

            await _service.DeleteProfile(3);

            _repoMock.Verify(r => r.DeleteProfileAsync(3), Times.Once);
        }

        // ────────────────────────────────────────────────────────────
        // ChangePassword Tests
        // ────────────────────────────────────────────────────────────

        [Fact]
        public async Task ChangePassword_ValidOldPassword_ShouldSucceed()
        {
            var oldHash = BCrypt.Net.BCrypt.HashPassword("oldpass");
            var user = new UserProfile
            {
                EmailId = "user@test.com",
                Password = oldHash,
                Addresses = new List<Address>()
            };
            _repoMock.Setup(r => r.FindByEmailIdAsync("user@test.com")).ReturnsAsync(user);
            _repoMock.Setup(r => r.UpdateProfileAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

            await _service.ChangePassword(new ChangePasswordDto
            {
                EmailId = "user@test.com",
                OldPassword = "oldpass",
                NewPassword = "newpass"
            });

            _repoMock.Verify(r => r.UpdateProfileAsync(It.IsAny<UserProfile>()), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_WrongOldPassword_ShouldThrowUnauthorized()
        {
            var user = new UserProfile
            {
                EmailId = "user@test.com",
                Password = BCrypt.Net.BCrypt.HashPassword("correctpass"),
                Addresses = new List<Address>()
            };
            _repoMock.Setup(r => r.FindByEmailIdAsync("user@test.com")).ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.ChangePassword(new ChangePasswordDto
                {
                    EmailId = "user@test.com",
                    OldPassword = "wrongpass",
                    NewPassword = "newpass"
                }));
        }
    }
}
