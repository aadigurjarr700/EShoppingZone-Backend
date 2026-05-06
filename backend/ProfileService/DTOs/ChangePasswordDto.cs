namespace ProfileService.DTOs
{
    public class ChangePasswordDto
    {
        public string EmailId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
