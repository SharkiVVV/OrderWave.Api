namespace OrderWaveAPI.Transfer.Responses;

public class WaiterResponse
{
    public int WaiterId { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }

    public bool IsOnShift { get; set; }
    public DateTime? ShiftStart { get; set; }

    public List<int> AssignedTables { get; set; } = new();
    
}