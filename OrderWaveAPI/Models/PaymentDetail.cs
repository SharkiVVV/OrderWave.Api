using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class PaymentDetail
{
    public int PaymentDetailId { get; set; }

    public int PaymentId { get; set; }

    public int OrderDetailId { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;
}
