using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileService.Entities
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        public int HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string ColonyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Pincode { get; set; }

        public int ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public UserProfile UserProfile { get; set; }
    }
}
