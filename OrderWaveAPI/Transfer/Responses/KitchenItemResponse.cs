namespace OrderWaveAPI.Transfer.Responses;

public class KitchenItemResponse
{
    public int QueueId { get; set; }
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public string DishName { get; set; } = null!;
    public int DishAmount { get; set; }
    
    public int TableNumber { get; set; }
    public string GuestName { get; set; } = null!;
    
    public string DishStatus { get; set; } = null!;
    public DateTime CreatedAt { get; set; }



}