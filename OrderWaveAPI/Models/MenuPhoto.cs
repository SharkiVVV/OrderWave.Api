using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class MenuPhoto
{
    public int PhotoId { get; set; }

    public int DishId { get; set; }

    public string PhotoUrl { get; set; } = null!;

    public bool IsMain { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Menu Dish { get; set; } = null!;
}
