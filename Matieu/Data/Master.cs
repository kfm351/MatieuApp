using System;
using System.Collections.Generic;

namespace Matieu.Data;

public partial class Master
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? QualificationLevel { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MasterService> MasterServices { get; set; } = new List<MasterService>();

    public virtual ICollection<QualificationRequest> QualificationRequests { get; set; } = new List<QualificationRequest>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? User { get; set; }
}
