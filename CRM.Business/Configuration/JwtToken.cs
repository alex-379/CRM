namespace CRM.Business.Configuration;

public class JwtToken
{
    public string ValidIssuer { get; private set; }
    public string ValidAudience { get; private set; }
    public int LifeTimeAccessToken { get; private set; }
    public int LifeTimeRefreshToken { get; private set; }
}
