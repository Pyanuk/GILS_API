using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Firstname { get; set; } = null!;

    public string? Lastname { get; set; }

    public DateOnly Birthday { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordUsers { get; set; }

    public int Countryid { get; set; }

    public int Cityid { get; set; }

    public int Roleid { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<FavoriteTrack> FavoriteTracks { get; set; } = new List<FavoriteTrack>();

    public virtual ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    public virtual ICollection<RecentlyPlayed> RecentlyPlayeds { get; set; } = new List<RecentlyPlayed>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Userauthprovider> Userauthproviders { get; set; } = new List<Userauthprovider>();
}
