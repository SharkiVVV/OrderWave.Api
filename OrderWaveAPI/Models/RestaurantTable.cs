using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class RestaurantTable
{
    public int TableId { get; set; }

    public int TableNumber { get; set; }

    public int TableCapacity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<TableSession> TableSessions{ get; set; }= new List<TableSession>();
}
