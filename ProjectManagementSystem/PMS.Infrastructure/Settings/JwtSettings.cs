namespace PMS.Infrastructure.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public double AccessTokenExpiryMinutes { get; set; }
    public double RefreshTokenExpiryDays { get; set; }
}
