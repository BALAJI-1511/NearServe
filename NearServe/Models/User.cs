namespace NearServe.Models
{
    public class User
    {
        public int CID { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PinHash { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? AccountNumber { get; set; }
        public decimal WalletBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
