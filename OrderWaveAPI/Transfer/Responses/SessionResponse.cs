namespace OrderWaveAPI.Transfer.Responses;

public class SessionResponse
{
    public int SessionId { get; set; }
    public int TableId { get; set; }
    public int TableNumber { get; set; }
    public bool IsActive { get; set; }
    public int GuestsAmout { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    
    public List<AssignedWaiterResponse> Waiters { get; set; }
    
}

public class AssignedWaiterResponse
{
    public int WaiterId { get; set; }   
    public string FirstName { get; set; }= null!;
    public string? LastName { get; set; }
    
}