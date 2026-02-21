namespace DevOpsReleasePortal.Auth;

public class JwtOptions
{
    public string Key { get; set; } = "ReplaceWithAStrongAndLongSecretKeyForProduction123!";
    public string Issuer { get; set; } = "DevOpsReleasePortal";
    public string Audience { get; set; } = "DevOpsReleasePortal.SPA";
    public int ExpiresMinutes { get; set; } = 60;
}
