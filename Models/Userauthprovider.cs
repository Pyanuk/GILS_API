using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Userauthprovider
{
    public int IdUserauthproviders { get; set; }

    public int Userid { get; set; }

    public string Providername { get; set; } = null!;

    public string Providerid { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
