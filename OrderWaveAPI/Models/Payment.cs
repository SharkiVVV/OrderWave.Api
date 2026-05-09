using System;
using System.Collections.Generic;

namespace OrderWaveAPI.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int SessionId { get; set; }

    public int? GuestId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Guest? Guest { get; set; }

    public virtual ICollection<PaymentDetail> PaymentDetails { get; set; } = new List<PaymentDetail>();

    public virtual TableSession Session { get; set; } = null!;
}
