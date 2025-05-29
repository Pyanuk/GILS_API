using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class City
{
    public int IdCity { get; set; }

    public string Cityname { get; set; } = null!;

    public int Countryid { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
