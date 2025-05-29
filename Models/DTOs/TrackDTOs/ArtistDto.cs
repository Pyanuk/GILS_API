namespace SoundPlayerAPI.Models.DTOs.TrackDTOs
{
    public class ArtistDto
    {
        public Guid ArtistId { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Photo { get; set; }

        public List<ConcertDto> Concerts { get; set; } = new();
    }
}
