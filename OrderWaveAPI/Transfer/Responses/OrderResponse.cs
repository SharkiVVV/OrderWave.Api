using OrderWaveAPI.Models;

namespace OrderWaveAPI.Transfer.Responses;

public class OrderResponse
{
    public int OrderId { get; set; }
    public int GuestId { get; set; }
    public string GuestName { get; set; } = null!;
    public int WaiterId { get; set; }
    public int SessionId { get; set; }
    public string CurrentStatus { get; set; } = null!;
    public DateTime OrderDate { get; set; }

    public List<OrderDetailResponse> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}
public class OrderDetailResponse
{
    public int OrderDetailId { get; set; }
    public int DishId { get; set; }
    public string DishName { get; set; } = null!;
    public int DishAmount { get; set; }
    
    public decimal DishPrice { get; set; }
    public decimal Subtotal { get; set; }
}