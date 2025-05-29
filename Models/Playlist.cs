using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Playlist
{
    public Guid PlaylistId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public int? TotalTracks { get; set; }

    public TimeSpan? TotalDuration { get; set; }

    public string? CoverPath { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
