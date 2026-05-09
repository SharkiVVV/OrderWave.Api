using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class OrderStatusHistory
{
    public int HistoryId { get; set; }

    public int OrderId { get; set; }

    public int ChangedBy { get; set; }

    public string Status { get; set; } = null!;

    public DateTime ChangedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User ChangedByNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
