using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class Waiter
{
    public int WaiterId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<TableAssignment> TableAssignments { get; set; } = new List<TableAssignment>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WaitersShift> WaitersShifts { get; set; } = new List<WaitersShift>();
}
