namespace TodoApp.Domain.UserAggregate;



public class AuthModel
{
    public bool HasError { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; } = false;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOn { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}