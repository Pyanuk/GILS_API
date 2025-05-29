using System.ComponentModel.DataAnnotations;

namespace SoundPlayerAPI.Models;

public partial class FavoriteTrack
{
    [Key]
    public int IdFavorite { get; set; } // Изменено с Id на IdFavorite
    public int UserId { get; set; }
    public Guid TrackId { get; set; }
    public virtual Track Track { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}