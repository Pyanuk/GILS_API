using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoundPlayerAPI.Models;

public partial class RecentlyPlayed
{
    [Key]
    public int RecentlyId { get; set; } 

    public int UserId { get; set; }

    public Guid TrackId { get; set; }


    public virtual Track Track { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
