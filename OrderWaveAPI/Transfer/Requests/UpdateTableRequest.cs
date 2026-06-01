namespace OrderWaveAPI.Transfer.Requests;

public class UpdateTableRequest
{
    public int TableNumber { get; set; }
    public int TableCapacity { get; set; }
    public bool IsActive { get; set; }
}