namespace GoogleLogin.Models.Api
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
