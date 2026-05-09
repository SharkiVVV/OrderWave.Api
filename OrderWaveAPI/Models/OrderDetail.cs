using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int DishId { get; set; }

    public int DishAmount { get; set; }

    public decimal DishPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Menu Dish { get; set; } = null!;

    public virtual ICollection<KitchenQueue> KitchenQueues { get; set; } = new List<KitchenQueue>();

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();
}
