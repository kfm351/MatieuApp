using System;
using System.Collections.Generic;

namespace Matieu.Data;

public partial class QualificationRequest
{
    public int Id { get; set; }

    public int? MasterId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Master? Master { get; set; }
}
