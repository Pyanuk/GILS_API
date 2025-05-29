using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Track
{
    public Guid TrackId { get; set; }

    public string Title { get; set; } = null!;

    public int? Duration { get; set; }

    public Guid? AlbumId { get; set; }

    public string? FilePath { get; set; }

    public string? CoverPath { get; set; }

    public string? Lyrics { get; set; }

    public string? Produsser { get; set; }

    public virtual Album? Album { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();
}
