using System;
using System.Collections.Generic;

namespace ProfileService.DTOs
{
    public class ProfileResponseDto
    {
        public int ProfileId { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string EmailId { get; set; }
        public long MobileNumber { get; set; }
        public string About { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public IList<AddressDto> Addresses { get; set; }
    }
}
