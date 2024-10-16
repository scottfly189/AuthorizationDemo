namespace AuthorizationDemo.Model
{
    public class AuthResponse
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string Token { get; set; }
        public string? ProfileImage { get; set; }
    }
}
