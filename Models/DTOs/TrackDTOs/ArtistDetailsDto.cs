using SoundPlayerAPI.Models.DTOs.TrackDTOs;

namespace SoundPlayerAPI.Models.DTOs.ArtistDTOs
{
    public class ArtistDetailsDto
    {
        public Guid ArtistId { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Photo { get; set; }
        public string? Type { get; set; }

        public List<string> Photos { get; set; } = new();
        public List<AlbumDto> Albums { get; set; } = new();
        public List<TrackDto> Tracks { get; set; } = new();
        public List<VideoDto> Videos { get; set; } = new();
        public List<SocialDto> Social { get; set; }

    }
}
