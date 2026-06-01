namespace OrderWaveAPI.Transfer.Requests;

public class CreateTableRequest
{
    public int TableNumber { get; set; }
    public int TableCapacity { get; set; }
}