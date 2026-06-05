namespace OrderWaveAPI.Transfer.Responses;

public class GuestResponse
{
    public int GuestId { get; set; }
    public string GuestName { get; set; } = null!;
    public string? GuestSurname { get; set; }
    
    public decimal TotalAmount { get; set; }
}