namespace NearServe.DTOs
{
    public class RegisterUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } // e.g., "Customer", "Admin","ServiceProvider"
    }
}
