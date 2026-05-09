using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int GuestId { get; set; }

    public int WaiterId { get; set; }

    public int SessionId { get; set; }

    public string CurrentStatus { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Guest Guest { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual TableSession Session { get; set; } = null!;

    public virtual Waiter Waiter { get; set; } = null!;
}
