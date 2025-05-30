using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class ArtistSocial
{
    public Guid Id { get; set; }

    public Guid Artistid { get; set; }

    public string? Platform { get; set; }

    public string? Url { get; set; }

    public string? Info { get; set; }

    public string? Title { get; set; }

    public virtual Artist Artist { get; set; } = null!;
}
