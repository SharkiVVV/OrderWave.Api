namespace OrderWaveAPI.Transfer.Requests;

public class CreateGuestRequest
{
    public string GuestName { get; set; } = null!;
    public string? GuestSurname { get; set; }
    
}