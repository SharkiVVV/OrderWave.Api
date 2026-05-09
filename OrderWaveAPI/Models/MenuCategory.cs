using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class MenuCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Menu> Menus { get; set; } = new List<Menu>();
}
