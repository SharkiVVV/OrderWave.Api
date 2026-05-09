using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class KitchenQueue
{
    public int QueueId { get; set; }

    public int OrderDetailId { get; set; }

    public string DishStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;
}
