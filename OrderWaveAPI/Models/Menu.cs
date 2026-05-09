using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class Menu
{
    public int DishId { get; set; }

    public int CategoryId { get; set; }

    public string DishName { get; set; } = null!;

    public decimal DishPrice { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual MenuCategory Category { get; set; } = null!;

    public virtual DishDetail? DishDetail { get; set; }

    public virtual MenuPhoto? MenuPhoto { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
