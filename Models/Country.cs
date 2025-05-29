using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Country
{
    public int IdCountry { get; set; }

    public string Countryname { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
