namespace GoogleLogin.Handlers
{
    public class PasswordHash
    {
        public static string HashPass(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        
        public static bool VerifyPass(string password, string hashedPassword)
        {    
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
