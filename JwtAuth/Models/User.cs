namespace JwtAuth.Models
{
    public partial class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;
        
        public string Role { get; set; } = "User"; // Default role
        
        public bool Isdeleted { get; set; }
    }
}
