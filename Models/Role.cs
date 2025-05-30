using System;
using System.Collections.Generic;

namespace SoundPlayerAPI.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
