namespace OrderWaveAPI.Transfer.Requests;

public class UpdateDishRequest
{
    public int CategoryId { get; set; }
    public string DishName { get; set; } = null!;
    public decimal DishPrice { get; set; }
    public bool IsActive { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Ingredients { get; set; }
    public string? Description { get; set; }
}