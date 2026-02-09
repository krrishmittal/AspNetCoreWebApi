namespace JwtAuth.Models.Api
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public LoginResponse? Data { get; set; }
        public string? ErrorMessage { get; set; }
        
        public static AuthenticationResult Successful(LoginResponse data)
        {
            return new AuthenticationResult
            {
                Success = true,
                Data = data
            };
        }
        
        public static AuthenticationResult Failed(string errorMessage)
        {
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}