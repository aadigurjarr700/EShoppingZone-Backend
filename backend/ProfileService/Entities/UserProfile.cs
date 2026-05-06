using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProfileService.Entities
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }
        public string FullName { get; set; }
        public string Image { get; set; }
        public string EmailId { get; set; }
        public long MobileNumber { get; set; }
        public string About { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }

        public IList<Address> Addresses { get; set; } = new List<Address>();
    }
}
