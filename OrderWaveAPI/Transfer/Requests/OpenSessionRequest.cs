namespace OrderWaveAPI.Transfer.Requests;

public class OpenSessionRequest
{
    public int GuestAmount { get; set; }
    public int WaiterId { get; set; }
}