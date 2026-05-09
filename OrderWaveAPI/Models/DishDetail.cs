using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class DishDetail
{
    public int DetailId { get; set; }

    public int DishId { get; set; }

    public string DishIngredients { get; set; } = null!;

    public string DishDescription { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Menu Dish { get; set; } = null!;
}
