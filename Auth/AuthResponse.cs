namespace DevOpsReleasePortal.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = [];
    public DateTime ExpiresAtUtc { get; set; }
}
