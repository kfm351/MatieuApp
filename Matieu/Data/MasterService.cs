using System;
using System.Collections.Generic;

namespace Matieu.Data;

public partial class MasterService
{
    public int Id { get; set; }

    public int? MasterId { get; set; }

    public int? ServiceId { get; set; }

    public virtual Master? Master { get; set; }

    public virtual Service? Service { get; set; }
}
