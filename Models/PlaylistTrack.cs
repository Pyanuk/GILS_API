using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class PlaylistTrack
{
    public Guid PlaylistId { get; set; }

    public Guid TrackId { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}
