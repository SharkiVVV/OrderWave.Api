namespace OrderWaveAPI.Transfer.Requests;

public class RegisterRequest
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    
    public string? Role { get; set; } = null!;

}