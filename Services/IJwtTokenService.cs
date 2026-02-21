using DevOpsReleasePortal.Data;

namespace DevOpsReleasePortal.Services;

public interface IJwtTokenService
{
    string CreateToken(ApplicationUser user, IList<string> roles, out DateTime expiresAtUtc);
}
