using System;
using System.Collections.Generic;

namespace EnglishCenterManagement_BackEnd.Models;

public partial class Receipt
{
    public int ReceiptId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public virtual StudentCourse StudentCourse { get; set; } = null!;
}
