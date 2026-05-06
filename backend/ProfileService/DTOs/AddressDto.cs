namespace ProfileService.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public int HouseNumber { get; set; }
        public string StreetName { get; set; }
        public string ColonyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Pincode { get; set; }
    }
}
