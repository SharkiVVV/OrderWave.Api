using OrderWaveAPI.Models;

namespace OrderWaveAPI.Transfer.Requests;

public class CreateOrderRequest
{
    public int GuestId { get; set; }
    public int WaiterId { get; set; }
    public int SessionId { get; set; }

    public List<OrderItemsRequest> Items { get; set; } = new();
    
}

public class OrderItemsRequest
{
    public int DishId { get; set; }
    public int Amount { get; set; }
}