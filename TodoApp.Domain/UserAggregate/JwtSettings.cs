namespace TodoApp.Domain.UserAggregate;

public sealed class JwtSettings
{
    public string Kid { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
}