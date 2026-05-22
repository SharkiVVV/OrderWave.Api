namespace OrderWaveAPI.Transfer.Responses;

public class DishDetailtResponse
{
    public int DishId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string DishName { get; set; } = null!;
    public decimal DishPrice { get; set; }
    public bool IsActive { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Ingredients { get; set; }
    public string? Description { get; set; }
}