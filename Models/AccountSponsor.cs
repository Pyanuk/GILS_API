using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class AccountSponsor
{
    public int IdAccountSponsor { get; set; }

    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;
}
