using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class ArtistPhoto
{
    public Guid Id { get; set; }

    public Guid ArtistId { get; set; }

    public string PhotoPath { get; set; } = null!;

    public bool? IsPrimary { get; set; }

    public virtual Artist Artist { get; set; } = null!;
}
