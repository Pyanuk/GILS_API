using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class RecentlyPlayed
{
    public int UserId { get; set; }

    public Guid TrackId { get; set; }

    public int RecentlyId { get; set; }

    public virtual Track Track { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
