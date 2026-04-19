using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matieu.Data;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string Category { get; set; } = null!;

    public int? CollectionId { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Collection? Collection { get; set; }

    public virtual ICollection<MasterService> MasterServices { get; set; } = new List<MasterService>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public string CollectionName => Collection?.Name ?? "Без коллекции";
    [NotMapped]
    public Bitmap? PreviewBitmap { get; set; }
}
