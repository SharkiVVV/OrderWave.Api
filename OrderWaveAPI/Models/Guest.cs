using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class Guest
{
    public int GuestId { get; set; }

    public int SessionId { get; set; }

    public string GuestName { get; set; } = null!;

    public string? GuestSurname { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual TableSession Session { get; set; } = null!;
}
