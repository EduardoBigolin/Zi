namespace Zi.Models.DTOs
{
    public class AuthDto
    {
        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
