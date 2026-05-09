using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class WaitersShift
{
    public int ShiftId { get; set; }

    public int WaiterId { get; set; }

    public DateTime ShiftStart { get; set; }

    public DateTime? ShiftEnd { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Waiter Waiter { get; set; } = null!;
}
