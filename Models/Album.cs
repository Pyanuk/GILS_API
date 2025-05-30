using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Album
{
    public Guid AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public string? CoverPath { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();
}
