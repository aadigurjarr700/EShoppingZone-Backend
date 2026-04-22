using System;
using System.Collections.Generic;

namespace ProfileService.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string EmailId { get; set; }
        public long MobileNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // CUSTOMER, MERCHANT, ADMIN
        public string Image { get; set; }
        public string About { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public IList<AddressDto> Addresses { get; set; }
    }
}
