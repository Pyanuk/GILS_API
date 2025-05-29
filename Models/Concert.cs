using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Concert
{
    public Guid ConcertId { get; set; }

    public string Title { get; set; } = null!;

    public string? Location { get; set; }

    public DateTime? Date { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();
}
