using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class TableSession
{
    public int SessionId { get; set; }

    public int TableId { get; set; }

    public bool IsActive { get; set; }

    public int GuestsAmount { get; set; }

    public DateTime OpenedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Guest> Guests { get; set; } = new List<Guest>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual RestaurantTable Table { get; set; } = null!;

    public virtual ICollection<TableAssignment> TableAssignments { get; set; } = new List<TableAssignment>();
}
