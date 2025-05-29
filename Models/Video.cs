using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Video
{
    public Guid VideoId { get; set; }

    public string? Title { get; set; }

    public Guid? TrackId { get; set; }

    public string? FilePath { get; set; }

    public string? VideoType { get; set; }

    public string? CoverPath { get; set; }

    public virtual Track? Track { get; set; }
}
