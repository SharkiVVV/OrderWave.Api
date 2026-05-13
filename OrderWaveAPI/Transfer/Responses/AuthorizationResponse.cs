namespace OrderWaveAPI.Transfer.Responses;

public class AuthorizationResponse
{
    public int UserId { get; set; }
    public string Login { get; set; } = null!;
    public string Role { get; set; } = null!;

    public string Token { get; set; } = null!;
}