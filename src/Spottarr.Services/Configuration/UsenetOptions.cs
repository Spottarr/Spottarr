namespace Spottarr.Services.Configuration;

public class UsenetOptions
{
    public const string Section = "Usenet";
    
    public string Hostname { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public int Port { get; init; }
    public bool UseTls { get; init; }
}