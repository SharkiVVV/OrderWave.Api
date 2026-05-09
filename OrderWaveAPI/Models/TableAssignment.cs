using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class TableAssignment
{
    public int AssignmentId { get; set; }

    public int SessionId { get; set; }

    public int WaiterId { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TableSession Session { get; set; } = null!;

    public virtual Waiter Waiter { get; set; } = null!;
}
