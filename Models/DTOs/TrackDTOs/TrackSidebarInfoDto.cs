namespace SoundPlayerAPI.Models.DTOs.TrackDTOs
{
    public class TrackSidebarInfoDto
    {
        public Guid TrackId { get; set; }
        public string Title { get; set; } = null!;
        public int? Duration { get; set; }

        public string? Lyrics { get; set; }
        public string? CoverPath { get; set; }
        public string? Produsser { get; set; }

        public List<ArtistDto> Artists { get; set; } = new();
        public List<VideoDto> Clips { get; set; } = new();
        public List<VideoDto> Samples { get; set; } = new();
    }
}
