using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Artist
{
    public Guid ArtistId { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? PhotoPath { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<Concert> Concerts { get; set; } = new List<Concert>();

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
